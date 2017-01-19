using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;

namespace PITCSurveyApp.Droid
{
    [Activity (Label = "We Count", Theme = "@style/MainTheme", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
	{
        protected override void OnCreate (Bundle bundle)
		{
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate (bundle);

            CrashManager.Register(this, UserSettings.HockeyAppId);

            MetricsManager.Register(Application, UserSettings.HockeyAppId);
            MetricsManager.EnableUserMetrics();

            global::Xamarin.Forms.Forms.Init (this, bundle);
            App.Init(this);
            LoadApplication (new PITCSurveyApp.App ());
		}

        public MobileServiceUser User
        {
            get { return SurveyCloudService.ApiClient.CurrentUser; }
            set { SurveyCloudService.ApiClient.CurrentUser = value; }
        }

        public Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters)
	    {
	        return SurveyCloudService.ApiClient.LoginAsync(this, provider, parameters);
	    }

	    public Task<MobileServiceUser> RefreshLoginAsync()
	    {
	        return SurveyCloudService.ApiClient.RefreshUserAsync();
        }

        public Task LogoutAsync()
        {
            CookieManager.Instance.RemoveAllCookie();
            return SurveyCloudService.ApiClient.LogoutAsync();
        }
    }
}

