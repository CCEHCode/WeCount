using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyApp.Views;
using PITCSurveyLib;
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
        private bool _isSurveyEnded;

        public SurveyViewModel()
            : this(new UploadedItem<SurveyResponseModel>(SurveyResponseModelExtensions.CreateNew()))
        {
        }

        public SurveyViewModel(UploadedItem<SurveyResponseModel> response)
        {
            _response = response;
            NextQuestionCommand = new Command(NextQuestion, () => CanGoForward);
            PreviousQuestionCommand = new Command(PreviousQuestion, () => CanGoBack);
            EditLocationCommand = new Command(EditLocation);
            Init();
        }

        public event EventHandler QuestionChanged;

        public Command NextQuestionCommand { get; }

        public Command PreviousQuestionCommand { get; }

        public Command EditLocationCommand { get; }

        public SurveyQuestionModel CurrentQuestion => Question(_index);

        public bool IsSurveyEnded => _isSurveyEnded;

        public IList<SurveyQuestionAnswerChoiceResponseModel> CurrentAnswers
        {
            get
            {
                var questionResponse = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == CurrentQuestion?.QuestionID);
                return questionResponse?.AnswerChoiceResponses ?? Array.Empty<SurveyQuestionAnswerChoiceResponseModel>();
            }
        }

        private bool CanGoForward => !_isSurveyEnded && CurrentAnswers.Count > 0;

        private bool CanGoBack => _index > 0;

        public int SurveyQuestionsCount => App.LatestSurvey?.Questions?.Count ?? 0;

        public Task UploadAsync()
        {
            return _response.UploadAsync();
        }

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
            PreviousQuestionCommand.ChangeCanExecute();
        }

        private async void NextQuestion()
        {
            await _response.SaveAsync();
            var currentAnswerIds = new HashSet<int>(CurrentAnswers.Select(a => a.AnswerChoiceID));
            var matchingAnswer = CurrentQuestion.AnswerChoices.FirstOrDefault(a => currentAnswerIds.Contains(a.AnswerChoiceID));
            var nextQuestionIndex = QuestionIndex(matchingAnswer?.NextQuestionID);
            // TODO: This is a hack because of a bug in the survey where the NextQuestionID produces a cycle. 
            // Confirm that question IDs are always increasing.
            _index = nextQuestionIndex > _index ? nextQuestionIndex : _index + 1;
            if ((matchingAnswer?.EndSurvey ?? false) || nextQuestionIndex < 0)
            {
                EndSurvey();
            }

            UpdateCommands();
            QuestionChanged?.Invoke(this, new EventArgs());
        }

        private async void PreviousQuestion()
        {
            await _response.SaveAsync();
            var previousId = int.MinValue;
            if (_isSurveyEnded)
            {
                _isSurveyEnded = false;
                previousId = _response.Item.QuestionResponses.Max(q => q.QuestionID);
            }
            else
            {
                var currentQuestion = CurrentQuestion;
                foreach (var response in _response.Item.QuestionResponses)
                {
                    if (response.QuestionID < currentQuestion.QuestionID && response.QuestionID > previousId)
                    {
                        previousId = response.QuestionID;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _index = QuestionIndex(previousId);
            UpdateCommands();
            QuestionChanged?.Invoke(this, new EventArgs());
        }

        private async void EditLocation()
        {
            await App.NavigationPage.Navigation.PushAsync(new SurveyLocationPage(_response, true));
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

        private void EndSurvey()
        {
            _isSurveyEnded = true;
            _response.Item.EndTime = DateTimeOffset.Now;
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

        private void Init()
        {
            var lastAnswer = _response.Item.QuestionResponses
                .Where(q => q.AnswerChoiceResponses.Count > 0)
                .MaxByOrDefault(a => a.QuestionID);

            if (lastAnswer == null)
            {
                _index = 0;
                return;
            }

            var lastQuestion = App.LatestSurvey?.Questions.FirstOrDefault(q => q.QuestionID == lastAnswer.QuestionID);
            if (lastQuestion == null)
            {
                _index = 0;
                return;
            }

            var lastAnswerIds = new HashSet<int>(lastAnswer.AnswerChoiceResponses.Select(a => a.AnswerChoiceID));
            var matchingAnswer = lastQuestion.AnswerChoices.FirstOrDefault(a => lastAnswerIds.Contains(a.AnswerChoiceID));
            var nextQuestionIndex = QuestionIndex(matchingAnswer?.NextQuestionID);
            var lastIndex = QuestionIndex(lastAnswer.QuestionID);
            _index = nextQuestionIndex > lastIndex ? nextQuestionIndex : lastIndex + 1;
            if ((matchingAnswer?.EndSurvey ?? false) || nextQuestionIndex < 0)
            {
                _isSurveyEnded = true;
            }
        }
    }
}
