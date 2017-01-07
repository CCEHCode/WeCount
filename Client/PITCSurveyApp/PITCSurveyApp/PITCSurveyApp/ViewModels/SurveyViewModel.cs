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
        private bool _endSurvey;

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

        public bool EndSurvey => _endSurvey;

        public IList<SurveyQuestionAnswerChoiceResponseModel> CurrentAnswers
        {
            get
            {
                var questionResponse = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == CurrentQuestion?.QuestionID);
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
            existingQuestion?.AnswerChoiceResponses.Remove(answer);
        }

        public void UpdateCommands()
        {
            NextQuestionCommand.ChangeCanExecute();
        }

        private async void NextQuestion()
        {
            await _fileHelper.SaveAsync(GetFilename(_response), _response);
            var currentAnswerIds = new HashSet<int>(CurrentAnswers.Select(a => a.AnswerChoiceID));
            var matchingAnswer = CurrentQuestion.AnswerChoices.FirstOrDefault(a => currentAnswerIds.Contains(a.AnswerChoiceID));
            var nextQuestionIndex = QuestionIndex(matchingAnswer?.NextQuestionID);
            // TODO: this is a hack because of a bug in the survey where the NextQuestionID produces a cycle
            _index = nextQuestionIndex > _index ? nextQuestionIndex : _index + 1;
            _endSurvey = (matchingAnswer?.EndSurvey ?? false) || nextQuestionIndex < 0;
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

        private int QuestionIndex(int? questionID)
        {
            var questions = App.LatestSurvey?.Questions;
            if (questions != null)
            {
                for (var i = 0; i < questions.Count; ++i)
                {
                    if (questions[i].QuestionID == questionID)
                    {
                        return i;
                    }
                }
            }

            return -1;
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
