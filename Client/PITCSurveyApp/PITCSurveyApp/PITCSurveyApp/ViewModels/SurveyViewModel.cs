using System;
using System.Collections.Generic;
using System.Linq;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    /// <summary>
    /// ViewModel class used to manage
    /// </summary>
    public class SurveyViewModel : BaseViewModel
    {
        private readonly IFileHelper _fileHelper = new FileHelper();
        private readonly UploadedItem<SurveyResponseModel> _response;

        private int _index;

        public SurveyViewModel()
            : this(CreateNewSurveyResponse())
        {
        }

        public SurveyViewModel(UploadedItem<SurveyResponseModel> response)
        {
            _response = response;
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
                var questionResponse = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == CurrentQuestion.QuestionID);
                return questionResponse?.AnswerChoiceResponses ?? Array.Empty<SurveyQuestionAnswerChoiceResponseModel>();
            }
        }

        private bool CanGoForward => CurrentAnswers.Count > 0;

        public int SurveyQuestionsCount => App.LatestSurvey?.Questions?.Count ?? 0;

        public void AddAnswer(SurveyQuestionAnswerChoiceResponseModel answer)
        {
            var existingQuestion = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == answer.QuestionID);
            if (existingQuestion == null)
            {
                existingQuestion = new SurveyQuestionResponseModel
                {
                    QuestionID = answer.QuestionID,
                };

                _response.Item.QuestionResponses.Add(existingQuestion);
            }

            existingQuestion.AnswerChoiceResponses.Add(answer);
        }
        
        public void RemoveAnswer(SurveyQuestionAnswerChoiceResponseModel answer)
        {
            var existingQuestion = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == answer.QuestionID);
            if (existingQuestion != null)
            {
                existingQuestion.AnswerChoiceResponses.Remove(answer);
            }
        }

        public void UpdateCommands()
        {
            NextQuestionCommand.ChangeCanExecute();
        }

        private async void NextQuestion()
        {
            await _fileHelper.SaveAsync(GetFilename(_response), _response);
            _index++;
            UpdateCommands();
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

        private static UploadedItem<SurveyResponseModel> CreateNewSurveyResponse()
        {
            return new UploadedItem<SurveyResponseModel>
            {
                Item = new SurveyResponseModel
                {
                    ResponseIdentifier = Guid.NewGuid(),
                    SurveyID = App.LatestSurvey.SurveyID,
                    Survey_Version = App.LatestSurvey.Version,
                    StartTime = DateTimeOffset.Now,
                },
            };
        }

        private static string GetFilename(UploadedItem<SurveyResponseModel> response)
        {
            return $"{response.Item.ResponseIdentifier}.survey.json";
        }
    }
}
