using System.Windows.Input;
using Xamarin.Forms;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Views;

namespace PITCSurveyApp.ViewModels
{
    class HomePageViewModel
    {
        private SurveyViewModel vm { get; set; }

        public ICommand NewSurveyCommand { get; set; }

        public HomePageViewModel()
        {
            NewSurveyCommand = new Command(NewSurvey);

            App.SurveyVM = new SurveyViewModel();      

            // TO DO: Need to populate this from the authentication service
            UserFullname = "Volunteer";
        }

        // Commands
        void NewSurvey(object obj)
        {
            App.NavigationPage.Navigation.PushAsync(new SurveyPage());
            App.MenuIsPresented = false;
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
    }
}
