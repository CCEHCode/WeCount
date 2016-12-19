using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
	public partial class SurveyPage : ContentPage
	{
	    private int _currentQuestionIndex;
        private Button _currentAnswer;

        public ICommand PreviousQuestionCommand { get; set; }
        public ICommand NextQuestionCommand { get; set; }
        public ICommand ButtonSelectedCommand { get; set; }

        public SurveyPage()
        {
            InitializeComponent();

            PreviousQuestionCommand = new Command(PreviousQuestion);
            NextQuestionCommand = new Command(NextQuestion);
            ButtonSelectedCommand = new Command(ButtonSelected);

            _currentQuestionIndex = 0;

            LoadQuestion(_currentQuestionIndex);
        }

	    void LoadQuestion(int index)
	    {
            try
            {
                // Access the current question
                SurveyQuestionModel cq = App.SurveyVM.Question(index);

                Title = $"Survey Question {cq.QuestionNum} of {App.SurveyVM.SurveyQuestionsCount.ToString()}";
                LblQuestion.Text = cq.QuestionText;
                LblHelpText.Text = cq.QuestionHelpText;

                //Need to clear the list of buttons in the current StackLayout
                StackAnswerOptions.Children.Clear();
                _currentAnswer = null;

                Button btn;
                foreach (SurveyQuestionAnswerChoiceModel answerOption in cq.AnswerChoices)
                {
                    btn = new Button();
                    btn.Text = $"{answerOption.AnswerChoiceNum} - {answerOption.AnswerChoiceText}";
                    //btn.HorizontalOptions = LayoutOptions.StartAndExpand;
                    btn.Command = ButtonSelectedCommand;
                    btn.CommandParameter = btn;
                    StackAnswerOptions.Children.Add(btn);
                }
            }
            catch (Exception ex)
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

        void ButtonSelected(object obj)
        {
            if (_currentAnswer != null)
            {
                _currentAnswer.BackgroundColor = Color.White;
            }

            Button btn = (Button)obj;
            btn.BackgroundColor = Color.Lime;
            _currentAnswer = btn;
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
