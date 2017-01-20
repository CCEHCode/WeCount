using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PITCSurveyApp.Models;
using PITCSurveyApp.Services;
using PITCSurveyApp.ViewModels;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
    public partial class SurveyPage : ContentPage
    {
        private readonly SurveyViewModel _viewModel;

        public SurveyPage()
            : this(new SurveyViewModel())
        {
        }

        public SurveyPage(UploadedItem<SurveyResponseModel> response)
            : this(new SurveyViewModel(response))
        {
        }

        private SurveyPage(SurveyViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.QuestionChanged += (sender, e) => UpdateQuestion();
            BindingContext = _viewModel;
            UpdateQuestion();
        }

	    private async void UpdateQuestion()
	    {
            // Reset the scroll view content
            AnswerOptionsScrollView.Content = null;

            // If the survey is ended, show the survey complete page
            if (_viewModel.IsSurveyEnded)
	        {
	            EndSurvey();
                return;
	        }

            // Otherwise, render the current question
	        var q = _viewModel.CurrentQuestion;
	        try
	        {
                // Update the title and question text
#if !WINDOWS_UWP
                Title = $"Survey Question {q.QuestionNum} of {_viewModel.MaximumQuestionNumber}";
                QuestionLabel.Text = q.QuestionText;
#else
                QuestionLabel.Text = $"{q.QuestionNum}. {q.QuestionText}";
#endif
	            HelpTextLabel.Text = q.QuestionHelpText;

                // Create a view for each of the answers
	            var answerOptionsStackLayout = new StackLayout();
                var choices = CreateChoices(q, _viewModel.CurrentAnswers);
	            foreach (var choice in choices)
	            {
#if __ANDROID__
                    // Android buttons support text wrapping, so we don't need to use the custom ContentButton 
                    var view = new SurveyAnswerItemAndroidView(choice);
#else
                    var view = new SurveyAnswerItemView(choice);
#endif
                    var stackLayout = new StackLayout();
	                stackLayout.Children.Add(view);
                    answerOptionsStackLayout.Children.Add(stackLayout);
	            }

                // Add the navigation button stack back to the answers stack layout
                // The navigation buttons should always come at the end of all the answers
                // to ensure the volunteer sees all the questions
	            answerOptionsStackLayout.Children.Add(NavigationButtonStackLayout);

	            AnswerOptionsScrollView.Content = answerOptionsStackLayout;

                // Return the scroll position to 0,0 for the answers list
                if (AnswerOptionsScrollView.ScrollY > 0)
                {
                    await AnswerOptionsScrollView.ScrollToAsync(0, 0, false);
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("SurveyQuestionFailed", ex);
                await DisplayAlert("Error", "Something went wrong when loading this question.", "OK");
	        }
	    }

        private async void EndSurvey()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("SurveyEnded");

            // Display the survey completion message
            Title = "Survey Complete";
            QuestionLabel.Text = "Thank you for participating.";
            HelpTextLabel.Text = "Uploading survey, please wait...";
            AnswerOptionsScrollView.Content = NavigationButtonStackLayout;

            try
            {
                // Upload and delete the survey upon completion
                await _viewModel.UploadAndDeleteAsync();
                HelpTextLabel.Text = $"Survey uploaded at {DateTime.Now.ToString("t", CultureInfo.CurrentCulture)}.";
            }
            catch
            {
                HelpTextLabel.Text = "Failed to upload survey, please try again from My Surveys menu page.";
            }
        }

        private IList<SurveyAnswerItemViewModel> CreateChoices(
            SurveyQuestionModel q, 
            IList<SurveyQuestionAnswerChoiceResponseModel> previousAnswers)
        {
            // Create the view model for each answer item, potentially using an existing answer response model
            var choices = new List<SurveyAnswerItemViewModel>(q.AnswerChoices.Count);
            foreach (var choice in q.AnswerChoices.OrderBy(a => a.AnswerChoiceNum, StringComparer.Ordinal))
            {
                var previousAnswer = previousAnswers.SingleOrDefault(a => a.AnswerChoiceID == choice.AnswerChoiceID);
                choices.Add(previousAnswer != null
                    ? new SurveyAnswerItemViewModel(choice, previousAnswer, true)
                    : new SurveyAnswerItemViewModel(
                        choice,
                        new SurveyQuestionAnswerChoiceResponseModel
                        {
                            QuestionID = q.QuestionID,
                            AnswerChoiceID = choice.AnswerChoiceID,
                        }));
            }

            // Add a listener for when the answer choice is selected
            foreach (var choice in choices)
            {
                // We reuse the PropertyChanged event since it's already wired up correctly
                choice.PropertyChanged += (sender, e) =>
                {
                    // Check to make sure it was the IsSelected property that changed
                    if (e.PropertyName == nameof(SurveyAnswerItemViewModel.IsSelected))
                    {
                        // If multiple answers are not allowed, unselect any other selected choices
                        if (!q.AllowMultipleAnswers && choice.IsSelected)
                        {
                            foreach (var otherChoice in choices)
                            {
                                if (otherChoice != choice)
                                {
                                    otherChoice.IsSelected = false;
                                }
                            }
                        }

                        // If the choice is selected, add it to the response model
                        if (choice.IsSelected)
                        {
                            _viewModel.AddAnswer(choice.Answer);
                        }
                        // Else when the choice is unselected, remove it from the response model
                        else
                        {
                            _viewModel.RemoveAnswer(choice.Answer);
                        }

                        _viewModel.UpdateCommands();
                    }
                };
            }

            return choices;
        }
    }
}
