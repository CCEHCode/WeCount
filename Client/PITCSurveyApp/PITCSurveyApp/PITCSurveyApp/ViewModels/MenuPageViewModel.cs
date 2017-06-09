using System.Windows.Input;
using PITCSurveyApp.Services;
using PITCSurveyApp.Views;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class MenuPageViewModel
    {
        public MenuPageViewModel()
        {
            GoHomeCommand = new Command(GoHome);
            GoMySurveysCommand = new Command(GoMySurveys);
            GoProfileCommand = new Command(GoProfile);
            GoContactInfoCommand = new Command(GoContactInfo);
        }

        public ICommand GoHomeCommand { get; }

        public ICommand GoMySurveysCommand { get; }

        public ICommand GoProfileCommand { get; }

        public ICommand GoContactInfoCommand { get; }

        void GoHome(object obj)
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("MenuGoHome");
            App.NavigationPage.Navigation.PopToRootAsync();
            App.MenuIsPresented = false;
        }

        void GoMySurveys(object obj)
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("MenuMySurveys");
            App.NavigationPage.Navigation.PushAsync(new MySurveysPage());
            App.MenuIsPresented = false;
        }

        void GoProfile(object obj)
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("MenuMyProfile");
            App.NavigationPage.Navigation.PushAsync(new ProfilePage());
            App.MenuIsPresented = false;
        }

        void GoContactInfo(object obj)
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("MenuContactInfo");
            App.NavigationPage.Navigation.PushAsync(new ContactInfoPage());
            App.MenuIsPresented = false;
        }
    }
}
