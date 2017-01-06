using System.Windows.Input;
using Xamarin.Forms;
using PITCSurveyApp.Views;

namespace PITCSurveyApp.ViewModels
{
    class MenuPageViewModel
    {
        public ICommand GoHomeCommand { get; set; }
        public ICommand GoMySurveysCommand { get; set; }
        public ICommand GoSettingsCommand { get; set; }

        public MenuPageViewModel()
        {
            GoHomeCommand = new Command(GoHome);
            GoMySurveysCommand = new Command(GoMySurveys);
            GoSettingsCommand = new Command(GoSettings);
        }

        void GoHome(object obj)
        {
            App.NavigationPage.Navigation.PopToRootAsync();
            App.MenuIsPresented = false;
        }

        void GoMySurveys(object obj)
        {
            App.NavigationPage.Navigation.PushAsync(new MySurveysPage());
            App.MenuIsPresented = false;
        }

        void GoSettings(object obj)
        {
            App.NavigationPage.Navigation.PushAsync(new SettingsPage());
            App.MenuIsPresented = false;
        }
    }
}
