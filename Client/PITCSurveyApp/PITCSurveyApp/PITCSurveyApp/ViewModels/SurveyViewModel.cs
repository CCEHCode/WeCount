using System;
using System.Collections.Generic;
using System.Linq;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    /// <summary>
    /// ViewModel class used to manage
    /// </summary>
    public class SurveyViewModel : BaseViewModel
    {
        private const string SurveyFileName = "questions.json";

        private readonly SurveyResponseModel _responses = new SurveyResponseModel();

        private int _index;

        public SurveyViewModel()
        {
            NextQuestionCommand = new Command(NextQuestion, () => CanGoForward);
            PreviousQuestionCommand = new Command(PreviousQuestion, () => false);
        }

        public event EventHandler QuestionChanged;

        public Command NextQuestionCommand { get; }

        public Command PreviousQuestionCommand { get; }

        public SurveyQuestionModel CurrentQuestion => Question(_index);

        public IList<SurveyQuestionAnswerChoiceResponseModel> CurrentAnswers
        {
            get
            {
                var questionResponse = _responses.QuestionResponses.FirstOrDefault(r => r.QuestionID == CurrentQuestion.QuestionID);
                return questionResponse?.AnswerChoiceResponses ?? Array.Empty<SurveyQuestionAnswerChoiceResponseModel>();
            }
        }

        private bool CanGoForward => CurrentAnswers.Count > 0;

        public int SurveyQuestionsCount => App.LatestSurvey?.Questions?.Count ?? 0;

        public void AddAnswer(SurveyQuestionAnswerChoiceResponseModel answer)
        {
            var existingQuestion = _responses.QuestionResponses.FirstOrDefault(r => r.QuestionID == answer.QuestionID);
            if (existingQuestion == null)
            {
                existingQuestion = new SurveyQuestionResponseModel
                {
                    QuestionID = answer.QuestionID,
                };

                _responses.QuestionResponses.Add(existingQuestion);
            }

            existingQuestion.AnswerChoiceResponses.Add(answer);
        }
        
        public void RemoveAnswer(SurveyQuestionAnswerChoiceResponseModel answer)
        {
            var existingQuestion = _responses.QuestionResponses.FirstOrDefault(r => r.QuestionID == answer.QuestionID);
            if (existingQuestion != null)
            {
                existingQuestion.AnswerChoiceResponses.Remove(answer);
            }
        }

        public void UpdateCommands()
        {
            NextQuestionCommand.ChangeCanExecute();
        }

        private void NextQuestion()
        {
            _index++;
            QuestionChanged?.Invoke(this, new EventArgs());
        }

        private void PreviousQuestion()
        {
            // TODO: use last answered question?
            throw new NotImplementedException();
        }

        private SurveyQuestionModel Question(int index)
        {
            var questions = App.LatestSurvey?.Questions;
            if (questions != null && index >= 0 && index < questions.Count)
            {
                return questions[index];
            }

            return null;
        }
    }
}
