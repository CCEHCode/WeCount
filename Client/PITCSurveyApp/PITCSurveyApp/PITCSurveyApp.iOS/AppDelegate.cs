using System.Threading.Tasks;
using Foundation;
using HockeyApp.iOS;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;
using UIKit;

namespace PITCSurveyApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        public MobileServiceUser User
        {
            get { return SurveyCloudService.ApiClient.CurrentUser; }
            set { SurveyCloudService.ApiClient.CurrentUser = value; }
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(Settings.HockeyAppId);
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation();
                // This line is obsolete in crash only builds (do we need it then?)

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new PITCSurveyApp.App());

            return base.FinishedLaunching(app, options);
        }

        public Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            return SurveyCloudService.ApiClient.LoginAsync(
                UIApplication.SharedApplication.KeyWindow.RootViewController,
                provider);
        }

        public Task RefreshLoginAsync()
        {
            return SurveyCloudService.ApiClient.RefreshUserAsync();
        }

        public Task LogoutAsync()
        {
            foreach (var cookie in NSHttpCookieStorage.SharedStorage.Cookies)
            {
                NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);
            }

            return SurveyCloudService.ApiClient.LogoutAsync();
        }
    }
}
