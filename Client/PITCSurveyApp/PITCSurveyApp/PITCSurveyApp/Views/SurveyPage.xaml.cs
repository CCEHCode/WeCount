using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using PITCSurveyApp.Models;
using PITCSurveyApp.ViewModels;
using PITCSurveyLib;
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

	    private void UpdateQuestion()
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
	            Title = $"Survey Question {q.QuestionNum} of {_viewModel.SurveyQuestionsCount}";
	            QuestionLabel.Text = q.QuestionText;
	            HelpTextLabel.Text = q.QuestionHelpText;

	            var answerOptionsStackLayout = new StackLayout();
                var choices = CreateChoices(q, _viewModel.CurrentAnswers);
	            foreach (var choice in choices)
	            {
	                var view = new SurveyAnswerItemView(choice);
	                var stackLayout = new StackLayout();
	                stackLayout.Children.Add(view);
                    answerOptionsStackLayout.Children.Add(stackLayout);
	            }

	            answerOptionsStackLayout.Children.Add(NavigationButtonStackLayout);

	            AnswerOptionsScrollView.Content = answerOptionsStackLayout;
	        }
	        catch
	        {
	            // TODO: provide better details, log in HockeyApp, etc.
	            DisplayAlert("Error", "Something went wrong when loading this question.", "OK");
	        }
	    }

        private async void EndSurvey()
        {
            Title = "Survey Complete";
            QuestionLabel.Text = "Thank you for participating.";
            HelpTextLabel.Text = "Uploading survey, please wait...";
            AnswerOptionsScrollView.Content = NavigationButtonStackLayout;

            try
            {
                await _viewModel.UploadAsync();
                HelpTextLabel.Text = $"Survey uploaded at {DateTime.Now.ToString("t", CultureInfo.CurrentCulture)}.";
            }
            catch (Exception ex)
            {
                HelpTextLabel.Text = "Failed to upload survey, please try again from My Surveys menu page.";
            }
        }

        private IList<SurveyAnswerItemViewModel> CreateChoices(
            SurveyQuestionModel q, 
            IList<SurveyQuestionAnswerChoiceResponseModel> previousAnswers)
        {
            var choices = new List<SurveyAnswerItemViewModel>(q.AnswerChoices.Count);
            foreach (var choice in q.AnswerChoices)
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
