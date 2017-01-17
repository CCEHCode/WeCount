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
            AnswerOptionsScrollView.Content = null;

            if (_viewModel.IsSurveyEnded)
	        {
	            EndSurvey();
                return;
	        }

	        var q = _viewModel.CurrentQuestion;
	        try
	        {
#if !WINDOWS_UWP
                Title = $"Survey Question {q.QuestionNum}";
#endif
                QuestionLabel.Text = q.QuestionText;
	            HelpTextLabel.Text = q.QuestionHelpText;

	            var answerOptionsStackLayout = new StackLayout();
                var choices = CreateChoices(q, _viewModel.CurrentAnswers);
	            foreach (var choice in choices)
	            {
#if __ANDROID__
                    var view = new SurveyAnswerItemAndroidView(choice);
#else
                    var view = new SurveyAnswerItemView(choice);
#endif
                    var stackLayout = new StackLayout();
	                stackLayout.Children.Add(view);
                    answerOptionsStackLayout.Children.Add(stackLayout);
	            }

	            answerOptionsStackLayout.Children.Add(NavigationButtonStackLayout);

	            AnswerOptionsScrollView.Content = answerOptionsStackLayout;

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

            Title = "Survey Complete";
            QuestionLabel.Text = "Thank you for participating.";
            HelpTextLabel.Text = "Uploading survey, please wait...";
            AnswerOptionsScrollView.Content = NavigationButtonStackLayout;

            try
            {
                await _viewModel.UploadAsync();
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
            var choices = new List<SurveyAnswerItemViewModel>(q.AnswerChoices.Count);
            foreach (var choice in q.AnswerChoices.OrderBy(a => a.AnswerChoiceNum))
            {
                var previousAnswer = previousAnswers.FirstOrDefault(a => a.AnswerChoiceID == choice.AnswerChoiceID);
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

            foreach (var choice in choices)
            {
                choice.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(SurveyAnswerItemViewModel.IsSelected))
                    {
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

                        if (choice.IsSelected)
                        {
                            _viewModel.AddAnswer(choice.Answer);
                        }
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
