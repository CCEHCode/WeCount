using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.ViewModels;
using PITCSurveyApp.Views;
using PITCSurveyLib;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Models
{
    class MySurveysItemViewModel : BaseViewModel
    {
        private readonly IFileHelper _fileHelper = new FileHelper();
        private readonly string _filename;

        private UploadedItem<SurveyResponseModel> _response;
        private DateTime? _lastModified;

        private Color _textColor;
        private string _text;
        private string _details;

        public MySurveysItemViewModel(string filename)
        {
            _filename = filename;
            DeleteCommand = new Command(Delete);
            UploadCommand = new Command(Upload);
            EditCommand = new Command(Edit);
        }

        public EventHandler Deleted;

        public Command DeleteCommand { get; }

        public Command UploadCommand { get; }

        public Command EditCommand { get; }

        public SurveyResponseModel Response => _response.Item;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }   
        }

        public string Details
        {
            get { return _details; }
            set { SetProperty(ref _details, value); }   
        }

        public Color TextColor
        {
            get { return _textColor; }
            set { SetProperty(ref _textColor, value); }
        }

        public bool IsIneligible
        {
            get
            {
                var lastResponse = _response.Item.QuestionResponses.MaxByOrDefault(r => r.QuestionID);
                if (lastResponse == null)
                {
                    return false;
                }

                var matchingQuestion = App.LatestSurvey?.Questions.FirstOrDefault(q => q.QuestionID == lastResponse.QuestionID);
                var lastResponseAnswerIds = new HashSet<int>(lastResponse.AnswerChoiceResponses.Select(r => r.AnswerChoiceID));
                var endSurveyAnswer = matchingQuestion?.AnswerChoices.FirstOrDefault(
                    a => lastResponseAnswerIds.Contains(a.AnswerChoiceID) && a.EndSurvey);
                return endSurveyAnswer != null;
            }
        }

        public DateTime? LastModified => _lastModified;

        private string Name => 
            _response.Item.GetWellKnownAnswer(App.LatestSurvey, WellKnownQuestion.NameOrInitials) ?? "Unknown";

        private string DateOfBirth =>
            _response.Item.GetWellKnownAnswer(App.LatestSurvey, WellKnownQuestion.DOB) ?? "Unknown";

        public async Task LoadAsync()
        {
            if (_filename != null)
            {
                _lastModified = await _fileHelper.LastModifiedAsync(_filename);
                _response = await _fileHelper.LoadAsync<UploadedItem<SurveyResponseModel>>(_filename);
                Update();
            }
        }

        public async Task UploadAsync()
        {
            await _response.UploadAsync();
            Update();
        }

        private async void Delete()
        {
            await _fileHelper.DeleteAsync(_filename);
            Deleted?.Invoke(this, new EventArgs());
        }

        private async void Upload()
        {
            try
            {
                await UploadAsync();
            }
            catch (Exception ex)
            {
                App.DisplayAlert(
                    "Upload Failure",
                    "Failed to upload survey, please try again.",
                    "OK");
            }
        }

        public async void Edit()
        {
            await App.NavigationPage.PushAsync(new SurveyPage(_response));
        }

        private void Update()
        {
            TextColor = _response.Uploaded.HasValue ? Color.Default : Color.Red;
            Text = IsIneligible ? "Ineligible Survey" : $"{Name}, {DateOfBirth}";
            Details = $"{PrettyPrintLastModified(_lastModified)}, {PrettyPrintUploaded(_response.Uploaded)}";
        }

        private static string PrettyPrintLastModified(DateTime? lastModified)
        {
            if (lastModified == null)
            {
                return "Not yet saved";
            }

            if (lastModified.Value.Date == DateTime.Now.Date)
            {
                return $"Last saved {lastModified.Value.ToString("t", CultureInfo.CurrentCulture)}";
            }
            else
            {
                return $"Last saved {lastModified.Value.ToString("g", CultureInfo.CurrentCulture)}";
            }
        }

        private static string PrettyPrintUploaded(DateTime? uploaded)
        {
            if (uploaded == null)
            {
                return "Not yet uploaded";
            }

            if (uploaded.Value.Date == DateTime.Now.Date)
            {
                return $"Uploaded {uploaded.Value.ToString("t", CultureInfo.CurrentCulture)}";
            }
            else
            {
                return $"Last saved {uploaded.Value.ToString("g", CultureInfo.CurrentCulture)}";
            }
        }
    }
}
