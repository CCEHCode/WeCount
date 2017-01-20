using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyApp.Services;
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

        private string _maxQuestion;
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
            NewSurveyCommand = new Command(NewSurvey);
            EditLocationCommand = new Command(EditLocation);
            Init();
        }

        public event EventHandler QuestionChanged;

        public Command NextQuestionCommand { get; }

        public Command PreviousQuestionCommand { get; }

        public Command NewSurveyCommand { get; }

        public Command EditLocationCommand { get; }

        public SurveyQuestionModel CurrentQuestion => Question(_index);

        public bool IsSurveyEnded
        {
            get { return _isSurveyEnded; }
            set
            {
                if (SetProperty(ref _isSurveyEnded, value))
                {
                    OnPropertyChanged(nameof(IsSurveyActive));
                }
            }
        }

        public bool IsSurveyActive => !_isSurveyEnded;

        public IList<SurveyQuestionAnswerChoiceResponseModel> CurrentAnswers
        {
            get
            {
                var questionResponse = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == CurrentQuestion?.QuestionID);
                return questionResponse?.AnswerChoiceResponses ?? Array.Empty<SurveyQuestionAnswerChoiceResponseModel>();
            }
        }

        private bool CanGoForward => !_isSurveyEnded && (CurrentQuestion.AllowMultipleAnswers || CurrentAnswers.Count > 0);

        private bool CanGoBack => _index > 0;

        //private Color backcolor = Color.Silver;

        public Color NextButtonBackColor
        {
            get { return CanGoForward ? Color.Green : Color.Silver; }
        }

        public string MaximumQuestionNumber
        {
            get
            {
                if (_maxQuestion == null)
                {
                    _maxQuestion = App.LatestSurvey?
                        .Questions?
                        .MaxByOrDefault(
                            q => q.QuestionNum, 
                            SurveyQuestionNumberComparer.Instance)?
                        .QuestionNum;
                }

                return _maxQuestion;
            }
        }

        public async Task UploadAndDeleteAsync()
        {
            await _response.UploadAsync();
            await _response.DeleteAsync();
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
            OnPropertyChanged("NextButtonBackColor");         
        }

        private async void NextQuestion()
        {
            // Add empty answers if necessary
            var questionResponse = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == CurrentQuestion?.QuestionID);
            if (questionResponse == null)
            {
                _response.Item.QuestionResponses.Add(new SurveyQuestionResponseModel
                {
                    QuestionID = CurrentQuestion.QuestionID,
                });
            }

            // Check if we should prompt for unspecified information
            var shouldPrompt = false;
            var questionAnswers = CurrentQuestion.AnswerChoices.ToDictionary(a => a.AnswerChoiceID, a => a);
            foreach (var answer in CurrentAnswers)
            {
                if (string.IsNullOrEmpty(answer.AdditionalAnswerData))
                {
                    var questionAnswer = default(SurveyQuestionAnswerChoiceModel);
                    if (questionAnswers.TryGetValue(answer.AnswerChoiceID, out questionAnswer) && questionAnswer.AdditionalAnswerDataFormat != AnswerFormat.None)
                    {
                        shouldPrompt = true;
                        break;
                    }
                }
            }

            if (shouldPrompt)
            {
                var shouldContinue = await App.DisplayAlertAsync(
                    "Incomplete Answer",
                    "At least one answer requires more information, are you sure you want to continue?", 
                    "Yes",
                    "No");

                if (!shouldContinue)
                {
                    return;
                }
            }

            var properties = new Dictionary<string, string>
            {
                {"CurrentQuestionID", CurrentQuestion?.QuestionID.ToString()},
            };

            DependencyService.Get<IMetricsManagerService>().TrackEvent("SurveyNextQuestion", properties, null);

            await _response.SaveAsync();
            var currentAnswerIds = new HashSet<int>(CurrentAnswers.Select(a => a.AnswerChoiceID));
            var matchingAnswer = CurrentQuestion.AnswerChoices.FirstOrDefault(a => currentAnswerIds.Contains(a.AnswerChoiceID));
            var nextQuestionIndex = QuestionIndex(matchingAnswer?.NextQuestionID) ?? NextQuestion(CurrentQuestion.QuestionNum);
            _index = nextQuestionIndex ?? _index + 1;
            if ((matchingAnswer?.EndSurvey ?? false) || nextQuestionIndex == null)
            {
                EndSurvey();
            }

            UpdateCommands();
            QuestionChanged?.Invoke(this, new EventArgs());
        }

        private async void PreviousQuestion()
        {
            var properties = new Dictionary<string, string>
            {
                {"CurrentQuestionID", CurrentQuestion?.QuestionID.ToString()},
            };

            DependencyService.Get<IMetricsManagerService>().TrackEvent("SurveyPreviousQuestion", properties, null);

            await _response.SaveAsync();
            var previousId = int.MinValue;
            if (_isSurveyEnded)
            {
                IsSurveyEnded = false;
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

            _index = QuestionIndex(previousId) ?? 0;
            UpdateCommands();
            QuestionChanged?.Invoke(this, new EventArgs());
        }

        private async void NewSurvey()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("SurveyPageNewSurvey");
            var currentPage = App.NavigationPage.CurrentPage;
            await App.NavigationPage.PushAsync(new SurveyLocationPage());
            App.NavigationPage.Navigation.RemovePage(currentPage);
        }

        private async void EditLocation()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("SurveyEditLocation");
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
            IsSurveyEnded = true;
            _response.Item.EndTime = DateTimeOffset.Now;
        }

        private int? QuestionIndex(int? questionId)
        {
            var questions = App.LatestSurvey?.Questions;
            if (questions != null)
            {
                for (var i = 0; i < questions.Count; ++i)
                {
                    if (questions[i].QuestionID == questionId)
                    {
                        return i;
                    }
                }
            }

            return null;
        }

        private int? NextQuestion(string currentQuestionNumber)
        {
            var allQuestions = App.LatestSurvey.Questions.OrderBy(q => q.QuestionNum, SurveyQuestionNumberComparer.Instance);
            using (var allQuestionsEnumerator = allQuestions.GetEnumerator())
            {
                while (allQuestionsEnumerator.MoveNext())
                {
                    if (allQuestionsEnumerator.Current.QuestionNum == currentQuestionNumber &&
                        allQuestionsEnumerator.MoveNext())
                    {
                        return QuestionIndex(allQuestionsEnumerator.Current.QuestionID);
                    }
                }
            }

            return null;
        }

        private void Init()
        {
            var lastAnswer = _response.Item.QuestionResponses
                .Where(q => q.AnswerChoiceResponses.Count > 0)
                .MaxByOrDefault(a => a.QuestionID);

            if (lastAnswer == null)
            {
                _index = FirstQuestionIndex();
                return;
            }

            var lastQuestion = App.LatestSurvey?.Questions.FirstOrDefault(q => q.QuestionID == lastAnswer.QuestionID);
            if (lastQuestion == null)
            {
                _index = FirstQuestionIndex();
                return;
            }

            var lastAnswerIds = new HashSet<int>(lastAnswer.AnswerChoiceResponses.Select(a => a.AnswerChoiceID));
            var matchingAnswer = lastQuestion.AnswerChoices.FirstOrDefault(a => lastAnswerIds.Contains(a.AnswerChoiceID));
            var nextQuestionIndex = QuestionIndex(matchingAnswer?.NextQuestionID) ?? NextQuestion(lastQuestion.QuestionNum);
            _index = nextQuestionIndex ?? 0;
            if ((matchingAnswer?.EndSurvey ?? false) || nextQuestionIndex == null)
            {
                IsSurveyEnded = true;
            }
        }

        private int FirstQuestionIndex()
        {
            return QuestionIndex(App.LatestSurvey?.Questions?.MinByOrDefault(q => q.QuestionNum).QuestionID) ?? 0;
        }
    }
}
