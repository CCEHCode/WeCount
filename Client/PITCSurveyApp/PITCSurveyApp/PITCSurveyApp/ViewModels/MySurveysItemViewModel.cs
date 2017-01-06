using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.ViewModels;
using Xamarin.Forms;

namespace PITCSurveyApp.Models
{
    class MySurveysItemViewModel : BaseViewModel
    {
        private readonly IFileHelper _fileHelper = new FileHelper();
        private readonly string _filename;

        private SurveyResponseModelWrapper _response;
        private DateTime? _lastModified;

        private Color _textColor;
        private string _details;

        public MySurveysItemViewModel(string filename, SurveyResponseModelWrapper response)
            : this(filename)
        {
            _response = response;
            Update();
        }

        public MySurveysItemViewModel(string filename)
        {
            _filename = filename;
            DeleteCommand = new Command(Delete);
            UploadCommand = new Command(async () => await UploadAsync());
        }

        public EventHandler Deleted;

        public Command DeleteCommand { get; }

        public Command UploadCommand { get; }

        public SurveyResponseModelWrapper Response => _response;

        public string Text => $"{_response.Name}, {_response.DateOfBirth}";

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

        public DateTime? LastModified => _lastModified;

        public async Task LoadAsync()
        {
            if (_filename != null)
            {
                _lastModified = await _fileHelper.LastModifiedAsync(_filename);

                var responseText = await _fileHelper.ReadTextAsync(_filename);
                var json = JObject.Parse(responseText);
                _response = json.ToObject<SurveyResponseModelWrapper>();
                Update();
            }
        }

        public async Task SaveAsync()
        {
            _lastModified = DateTime.Now;

            var json = JObject.FromObject(_response);
            var responseText = json.ToString(Formatting.None);
            await _fileHelper.WriteTextAsync(_filename, responseText);
        }

        public async Task UploadAsync()
        {
            // TODO: perform upload and capture metrics
            _response.Uploaded = DateTime.Now;
            await SaveAsync();
            Update();
        }

        private async void Delete()
        {
            await _fileHelper.DeleteAsync(_filename);
            Deleted?.Invoke(this, new EventArgs());
        }

        private void Update()
        {
            TextColor = _response.Uploaded.HasValue ? Color.Default : Color.Red;
            Details = $"{PrettyPrintLastModified(LastModified)}, {PrettyPrintUploaded(_response.Uploaded)}";
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
