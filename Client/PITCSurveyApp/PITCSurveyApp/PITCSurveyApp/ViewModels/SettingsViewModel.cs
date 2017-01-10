using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Helpers;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            MicrosoftLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.MicrosoftAccount));
            GoogleLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.Google));
            LogoutCommand = new Command(Logout);
        }

        public Command MicrosoftLoginCommand { get; }

        public Command GoogleLoginCommand { get; }

        public Command LogoutCommand { get; }

        public bool IsAnonymous => !Settings.IsLoggedIn;

        public bool IsLoggedIn => Settings.IsLoggedIn;

        private async void SignIn(MobileServiceAuthenticationProvider provider)
        {
            await App.LoginAsync(provider);
            OnPropertyChanged(nameof(IsAnonymous));
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        private async void Logout()
        {
            await App.LogoutAsync();
            OnPropertyChanged(nameof(IsAnonymous));
            OnPropertyChanged(nameof(IsLoggedIn));
        }
    }
}
