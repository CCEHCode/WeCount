using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using HockeyApp.Android;
using HockeyApp.Android.Metrics;

using PITCSurveyApp.Helpers;

namespace PITCSurveyApp.Droid
{
	[Activity (Label = "PITC Survey App", Theme = "@style/MainTheme", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate (bundle);

            CrashManager.Register(this, Settings.HockeyAppId);

            MetricsManager.Register(Application, Settings.HockeyAppId);
            MetricsManager.EnableUserMetrics();

            global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new PITCSurveyApp.App ());
		}
	}
}

