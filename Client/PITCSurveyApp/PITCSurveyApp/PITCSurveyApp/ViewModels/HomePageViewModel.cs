using System;
using System.Windows.Input;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;
using PITCSurveyApp.Views;
using PITCSurveyLib.Models;
using Xamarin.Forms;

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

        public string UserGreeting => "Welcome Volunteer";

        public ImageSource BannerImage => ImageSource.FromFile(CrossHelper.GetOSFullImagePath("ccehlogo.jpg"));

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
            // Check if the survey has already been downloaded
            var fileHelper = new FileHelper();
            if (await fileHelper.ExistsAsync(SurveyFileName))
            {
                // Load the survey if it exists
                App.LatestSurvey = await fileHelper.LoadAsync<SurveyModel>(SurveyFileName);
            }

            try
            {
                // Check if an updated survey is available from the service
                var azureSurvey = await SurveyCloudService.GetSurveyAsync(1); // TODO: Replace with actual SurveyID, from GetAvailableSurveysAsync()
                if (azureSurvey != null && (App.LatestSurvey == null || App.LatestSurvey.Version < azureSurvey.Version))
                {
                    App.LatestSurvey = azureSurvey;
                    // Save the survey to the local filesystem
                    await fileHelper.SaveAsync(SurveyFileName, azureSurvey);
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
