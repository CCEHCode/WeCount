using System;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;
using PITCSurveyApp.Views;
using PITCSurveyLib.Models;
using Xamarin.Forms;
using PITCSurveyLib;

namespace PITCSurveyApp.ViewModels
{
    class HomePageViewModel : BaseViewModel
    {
        private const string SurveyFileName = "questions.json";

        private string _surveyVersion = "Loading...";
        private string _surveyQuestionCount = "Loading...";

        public HomePageViewModel()
        {
            NewSurveyCommand = new Command(NewSurvey);
            LoadSurveyCommand = new Command(LoadSurvey);
            IsBusy = true;
            Init();

            // TODO: Need to populate this from the authentication service
            UserFullname = "Volunteer";
        }

        public ICommand NewSurveyCommand { get; }

        public ICommand LoadSurveyCommand { get; }

        public string SurveyVersion
        {
            get { return _surveyVersion; }
            set { SetProperty(ref _surveyVersion, value); }
        }

        public string SurveyQuestionCount
        {
            get { return _surveyQuestionCount; }
            set { SetProperty(ref _surveyQuestionCount, value); }
        }

        public string UserFullname { get; set; }

        public string UserGreeting
        {
            get { return "Welcome " + UserFullname; }
        }

        public ImageSource BannerImage
        {
            get { return ImageSource.FromFile(CrossHelper.GetOSFullImagePath("ccehlogo.jpg")); }
        }

        private void NewSurvey(object obj)
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("HomePageNewSurvey");
            App.NavigationPage.Navigation.PushAsync(new SurveyLocationPage());
        }

        private void LoadSurvey(object obj)
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("HomePageLoadSurvey");
            App.NavigationPage.Navigation.PushAsync(new MySurveysPage(true));
        }

        private async void Init()
        {
            var fileHelper = new FileHelper();
            if (await fileHelper.ExistsAsync(SurveyFileName))
            {
                var surveyText = await fileHelper.ReadTextAsync(SurveyFileName);
                var surveyJson = JObject.Parse(surveyText);
                App.LatestSurvey = surveyJson.ToObject<SurveyModel>();
            }

            try
            {
                // TODO: add logic to only periodically check for survey updates
                var azureSurvey = await SurveyCloudService.GetSurveyAsync(1); // TODO: Replace with actual SurveyID, from GetAvailableSurveysAsync()
                if (App.LatestSurvey == null || App.LatestSurvey.Version < azureSurvey.Version)
                {
                    App.LatestSurvey = azureSurvey;
                    var surveyJson = JObject.FromObject(azureSurvey);
                    var surveyText = surveyJson.ToString(Formatting.None);
                    await fileHelper.WriteTextAsync(SurveyFileName, surveyText);
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("GetSurveyFailed", ex);
            }

            IsBusy = false;
            UpdateSurveyInfo();
        }

        private void UpdateSurveyInfo()
        {
            var survey = App.LatestSurvey;
            if (survey == null)
            {
                SurveyVersion = "Failed to load survey.";
                SurveyQuestionCount = "Failed to load survey.";
            }
            else
            {
                SurveyVersion = survey.Version.ToString();
                SurveyQuestionCount = survey.Questions?.Count.ToString() ?? "Invalid survey.";
            }
        }
    }
}
