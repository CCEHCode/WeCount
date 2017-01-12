using System.Windows.Input;
using Xamarin.Forms;
using PITCSurveyApp.Views;

namespace PITCSurveyApp.ViewModels
{
    class MenuPageViewModel
    {
        public ICommand GoHomeCommand { get; set; }
        public ICommand GoMySurveysCommand { get; set; }
        public ICommand GoProfileCommand { get; set; }

        public MenuPageViewModel()
        {
            GoHomeCommand = new Command(GoHome);
            GoMySurveysCommand = new Command(GoMySurveys);
            GoProfileCommand = new Command(GoProfile);
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

        void GoProfile(object obj)
        {
            App.NavigationPage.Navigation.PushAsync(new ProfilePage());
            App.MenuIsPresented = false;
        }
    }
}
