using System;
using System.Collections.Generic;
using System.Linq;
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
    class SurveyViewModel : BaseViewModel
    {
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
            NewSurveyCommand = new Command(NewSurvey, () => IsNotBusy);
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

        /// <summary>
        /// The answers in the survey response that match the current question identifier.
        /// </summary>
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

        public Color NextButtonBackColor => CanGoForward ? Color.Green : Color.Silver;

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
            IsBusy = true;
            NewSurveyCommand.ChangeCanExecute();

            try
            {
                await _response.UploadAsync();
                await _response.DeleteAsync();
            }
            finally
            {
                IsBusy = false;
                NewSurveyCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        /// Adds a selected answer to the survey response model.
        /// </summary>
        /// <param name="answer">The answer to add.<param>
        public void AddAnswer(SurveyQuestionAnswerChoiceResponseModel answer)
        {
            // Check if a question response model has already been created for the question
            var existingQuestion = _response.Item.QuestionResponses.FirstOrDefault(r => r.QuestionID == answer.QuestionID);
            if (existingQuestion == null)
            {
                // If not yet created, create a new question response model
                existingQuestion = new SurveyQuestionResponseModel
                {
                    QuestionID = answer.QuestionID,
                };

                _response.Item.QuestionResponses.Add(existingQuestion);
            }

            var existingAnswer = existingQuestion.AnswerChoiceResponses.FirstOrDefault(a => a.AnswerChoiceID == answer.AnswerChoiceID);
            if (existingAnswer == null)
            {
                // Add the answer to the question response model
                existingQuestion.AnswerChoiceResponses.Add(answer);
            }
            else
            {
                // If the answer already exists, we should report this (not expected behavior)
                var properties = new Dictionary<string, string>
                {
                    { "QuestionID", answer.QuestionID.ToString() },
                    { "AnswerChoiceID", answer.AnswerChoiceID.ToString() },
                };

                DependencyService.Get<IMetricsManagerService>().TrackEvent("DuplicateAnswer", properties, null);
            }
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
            OnPropertyChanged(nameof(NextButtonBackColor));         
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

            // Check if we should prompt in case the user did not fill out the additional information field
            var shouldPrompt = false;
            var questionAnswers = CurrentQuestion.AnswerChoices.ToDictionary(a => a.AnswerChoiceID, a => a);
            foreach (var answer in CurrentAnswers)
            {
                if (string.IsNullOrEmpty(answer.AdditionalAnswerData))
                {
                    SurveyQuestionAnswerChoiceModel questionAnswer;
                    if (questionAnswers.TryGetValue(answer.AnswerChoiceID, out questionAnswer) && questionAnswer.AdditionalAnswerDataFormat != AnswerFormat.None)
                    {
                        shouldPrompt = true;
                        break;
                    }
                }
            }

            // Prompt the user if any additional information entries are empty
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

            // Save the response
            await _response.SaveAsync();

            // Create a lookup table for all the currently selected answers
            var currentAnswerIds = new HashSet<int>(CurrentAnswers.Select(a => a.AnswerChoiceID));
            
            // Get a reference to any matching question answer model from the survey question
            var matchingAnswer = CurrentQuestion.AnswerChoices.FirstOrDefault(a => currentAnswerIds.Contains(a.AnswerChoiceID));
            
            // Set the next question to the next question identifier from the question answer model if available;
            // otherwise, set the next question to the next highest question number
            var nextQuestionIndex = QuestionIndex(matchingAnswer?.NextQuestionID) ?? NextQuestion(CurrentQuestion.QuestionNum);

            _index = nextQuestionIndex ?? _index + 1;

            // End the survey if the matching question answer disqualifies the survey, or if there are no more questions
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

            // Save the response
            await _response.SaveAsync();

            // If the survey is ended, set the question to the last answered question
            int? previousId;
            if (_isSurveyEnded)
            {
                IsSurveyEnded = false;
                previousId = _response.Item.QuestionResponses.Max(q => q.QuestionID);
            }
            else
            {
                // Otherwise, set the question to the question with the next lowest question number
                previousId = PreviousQuestion(CurrentQuestion.QuestionNum);
            }

            // If the question identifier cannot be found, set to the first question (should not happen)
            _index = QuestionIndex(previousId) ?? FirstQuestionIndex();
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

        /// <summary>
        /// Find the index for the given question identifier.
        /// </summary>
        /// <param name="questionId">The question identifier.</param>
        /// <returns>The question index.</returns>
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

        /// <summary>
        /// Find the index of the question that comes next, ordered by question number.
        /// </summary>
        /// <param name="currentQuestionNumber">The current question number.</param>
        /// <returns>The index of the next question.</returns>
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

        /// <summary>
        /// Find the identifier of the question that comes before the current
        /// question, ordered by question number.
        /// </summary>
        /// <param name="currentQuestionNumber">The current question.</param>
        /// <returns>The previous question identifier.</returns>
        private int? PreviousQuestion(string currentQuestionNumber)
        {
            // Get the sorted list of question numbers
            var allQuestions = App.LatestSurvey.Questions.OrderByDescending(q => q.QuestionNum, SurveyQuestionNumberComparer.Instance);

            // Get a lookup table for all answered question IDs
            var answeredQuestionIds = new HashSet<int>(_response.Item.QuestionResponses.Select(q => q.QuestionID));

            using (var allQuestionsEnumerator = allQuestions.GetEnumerator())
            {
                // Start enumerating through the question numbers
                while (allQuestionsEnumerator.MoveNext())
                {
                    // Once we find the current question number, move to the next question
                    if (allQuestionsEnumerator.Current.QuestionNum == currentQuestionNumber &&
                        allQuestionsEnumerator.MoveNext())
                    {
                        // Keep moving to the next question until we find an answered question
                        var moveNext = true;
                        while (!answeredQuestionIds.Contains(allQuestionsEnumerator.Current.QuestionID) && moveNext)
                        {
                            moveNext = allQuestionsEnumerator.MoveNext();
                        }

                        // If we didn't reach the end of the enumerator, return the current question ID.
                        if (moveNext)
                        {
                            return allQuestionsEnumerator.Current.QuestionID;
                        }

                        break;
                    }
                }
            }

            return null;
        }

        private void Init()
        {
            // Find the last question response
            var lastAnswer = _response.Item.QuestionResponses
                .Where(q => q.AnswerChoiceResponses.Count > 0)
                .MaxByOrDefault(a => a.QuestionID);

            // If no questions were answered, start the survey from the first question
            if (lastAnswer == null)
            {
                _index = FirstQuestionIndex();
                return;
            }

            // Find the matching survey question that goes with the last response
            var lastQuestion = App.LatestSurvey?.Questions.FirstOrDefault(q => q.QuestionID == lastAnswer.QuestionID);

            // If the survey question cannot be found, start the survey from the first question
            if (lastQuestion == null)
            {
                _index = FirstQuestionIndex();
                return;
            }

            // Create a lookup of answer choice IDs from the last response
            var lastAnswerIds = new HashSet<int>(lastAnswer.AnswerChoiceResponses.Select(a => a.AnswerChoiceID));

            // Find the first question answer choice that matches a selected answer ID.
            var matchingAnswer = lastQuestion.AnswerChoices.FirstOrDefault(a => lastAnswerIds.Contains(a.AnswerChoiceID));

            // Start the survey from the next question based on the users answer choice, or the next question sequentially.
            var nextQuestionIndex = QuestionIndex(matchingAnswer?.NextQuestionID) ?? NextQuestion(lastQuestion.QuestionNum);
            _index = nextQuestionIndex ?? 0;

            // Check if the survey has been completed based on the survey's last answer choice, or if there are no more questions
            if ((matchingAnswer?.EndSurvey ?? false) || nextQuestionIndex == null)
            {
                IsSurveyEnded = true;
            }
        }

        /// <summary>
        /// Get the index of the question with the lowest question number.
        /// </summary>
        /// <returns>The question index.</returns>
        private int FirstQuestionIndex()
        {
            return QuestionIndex(App.LatestSurvey?.Questions?.MinByOrDefault(q => q.QuestionNum).QuestionID) ?? 0;
        }
    }
}
