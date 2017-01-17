using System.Windows.Input;
using PITCSurveyApp.Services;
using Xamarin.Forms;
using PITCSurveyApp.Views;

namespace PITCSurveyApp.ViewModels
{
    class MenuPageViewModel
    {
        public ICommand GoHomeCommand { get; set; }
        public ICommand GoMySurveysCommand { get; set; }
        public ICommand GoProfileCommand { get; set; }
		public ICommand GoContactInfoCommand { get; set; }

		public MenuPageViewModel()
        {
            GoHomeCommand = new Command(GoHome);
            GoMySurveysCommand = new Command(GoMySurveys);
            GoProfileCommand = new Command(GoProfile);
			GoContactInfoCommand = new Command(GoContactInfo);
        }

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
