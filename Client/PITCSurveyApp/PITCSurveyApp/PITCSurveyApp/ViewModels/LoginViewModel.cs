using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Helpers;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            IsBusy = true;
            MicrosoftLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.MicrosoftAccount), () => IsNotBusy);
            GoogleLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.Google), () => IsNotBusy);
            NotNowCommand = new Command(App.GoToMainPage, () => IsNotBusy);
            RefreshUser();
        }

        public ImageSource UserImage => 
            ImageSource.FromFile(
                CrossHelper.GetOSFullImagePath("profile_generic.png"));

        public Command NotNowCommand { get; }

        public Command GoogleLoginCommand { get; }

        public Command MicrosoftLoginCommand { get; }

        private async void SignIn(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                await App.LoginAsync(provider);
            }
            finally
            {
#if WINDOWS_UWP
                // Updating the main page here will cause an exception for Android
                // Instead, for Android and iOS, we wait for the view to send the
                // `OnAppearing` event and update the main page then.
                if (Settings.IsLoggedIn)
                {
                    App.GoToMainPage();
                }
#endif
            }
        }

        private async void RefreshUser()
        {
            await App.RefreshLoginAsync();

            Settings.Initializing = false;

            if (Settings.IsLoggedIn)
            {
                App.GoToMainPage();
            }
            else
            {
                IsBusy = false;
                MicrosoftLoginCommand.ChangeCanExecute();
                GoogleLoginCommand.ChangeCanExecute();
                NotNowCommand.ChangeCanExecute();
            }
        }
    }
}