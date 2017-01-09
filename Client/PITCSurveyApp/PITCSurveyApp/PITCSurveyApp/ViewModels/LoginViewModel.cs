using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            SignInCommand = new Command(async () => await SignIn());
            NotNowCommand = new Command(App.GoToMainPage);
        }

        public ImageSource UserImage => 
            ImageSource.FromFile(
                CrossHelper.GetOSFullImagePath("profile_generic.png"));

        public ICommand NotNowCommand { get; }

        public ICommand SignInCommand { get; }

        private async Task SignIn()
        {
            try
            {
                DependencyService.Get<IMetricsManagerService>().TrackEvent("UserLogin");
                await App.Authenticator.AuthenticateAsync(MobileServiceAuthenticationProvider.Google);
            }
            finally
            {
                if (Settings.IsLoggedIn)
                {
                    App.GoToMainPage();
                }
            }
        }
    }
}