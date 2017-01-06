using System;
using System.Windows.Input;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
    public partial class SurveyPage : ContentPage
	{
	    private int _currentQuestionIndex;

        public ICommand PreviousQuestionCommand { get; set; }
        public ICommand NextQuestionCommand { get; set; }

        public SurveyPage()
        {
            InitializeComponent();

            PreviousQuestionCommand = new Command(PreviousQuestion);
            NextQuestionCommand = new Command(NextQuestion);

            _currentQuestionIndex = 0;

            LoadQuestion(_currentQuestionIndex);
        }

	    void LoadQuestion(int index)
	    {
            try
            {
                // Access the current question
                SurveyQuestionModel cq = App.SurveyVM.Question(index);

                // BUG: For some reason, the title is not updating after question 1 on UWP, not tested on other platforms yet
                Title = $"Survey Question {cq.QuestionNum} of {App.SurveyVM.SurveyQuestionsCount.ToString()}";
                LblQuestion.Text = cq.QuestionText;
                LblHelpText.Text = cq.QuestionHelpText;

                AnswersList.ItemsSource = cq.AnswerChoices;
                // Trying to force layout on the page to update the title, but has no effect
                //this.ForceLayout();  
            }
            catch
            {
                // TO DO: provide better details, log in HockeyApp, etc.
                DisplayAlert("Error", "Something went wrong when loading this question.", "OK");
            }
        }

	    void PreviousQuestion(object obj)
	    {
            // Not implemented yet
            throw new NotImplementedException();
        }

	    void NextQuestion(object obj)
	    {
            // This is temporary, each question/answer will actually specify which question comes next
            _currentQuestionIndex++;

            LoadQuestion(_currentQuestionIndex);
        }

	    private void PreviousButton_OnClicked(object sender, EventArgs e)
	    {
	        PreviousQuestion(null);
	    }

	    private void NextButton_OnClicked(object sender, EventArgs e)
	    {
	        NextQuestion(null);
	    }
	}
}
