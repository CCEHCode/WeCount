using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            IsBusy = true;
            MicrosoftLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.MicrosoftAccount), () => IsNotBusy);
            GoogleLoginCommand = new Command(() => SignIn(MobileServiceAuthenticationProvider.Google), () => IsNotBusy);
            NotNowCommand = new Command(SkipLogin, () => IsNotBusy);
            RefreshUser();
        }

        public ImageSource UserImage => 
            ImageSource.FromFile(
                CrossHelper.GetOSFullImagePath("profile_generic.png"));

        public Command NotNowCommand { get; }

        public Command GoogleLoginCommand { get; }

        public Command MicrosoftLoginCommand { get; }

#if __IOS__
        public bool IsGoogleLoginAvailable => false;
#else
        public bool IsGoogleLoginAvailable => true;
#endif

        private void SkipLogin()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("LoginPageSkipLogin");
            App.GoToMainPage();
        }

        private async void SignIn(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                IsBusy = true;

                var properties = new Dictionary<string, string>
                {
                    { "LoginProvider", provider.ToString() }
                };

                DependencyService.Get<IMetricsManagerService>().TrackEvent("LoginPageLogin", properties, null);
                await App.LoginAsync(provider);
            }
            finally
            {
#if !__ANDROID__
                // Updating the main page here will cause an exception for Android
                // Instead, for Android and iOS, we wait for the view to send the
                // `OnAppearing` event and update the main page then.
                if (UserSettings.IsLoggedIn)
                {
                    App.GoToMainPage();
                }

#endif
                IsBusy = false;
            }
        }

        private async void RefreshUser()
        {
            UserSettings.IsRefreshingAuthToken = true;

            try
            {
                await App.RefreshLoginAsync();
            }
            finally
            {
                UserSettings.IsRefreshingAuthToken = false;
            }

            if (UserSettings.IsLoggedIn)
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