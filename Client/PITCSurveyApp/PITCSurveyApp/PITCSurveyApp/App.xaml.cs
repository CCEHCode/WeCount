using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Views;

namespace PITCSurveyApp
{
	public partial class App : Application
	{
        public static string AzureMobileAppUrl = "https://pitcsurveyapi.azurewebsites.net";
        public static IDictionary<string, string> LoginParameters => null;

        public static NavigationPage NavigationPage { get; private set; }
        private static RootPage RootPage;

        public static bool MenuIsPresented
        {
            get
            {
                return RootPage.IsPresented;
            }
            set
            {
                RootPage.IsPresented = value;
            }
        }

        public App ()
		{
			InitializeComponent();

            DependencyService.Get<IMetricsManagerService>().TrackEvent("AppStarted");

            SetMainPage();
        }

        public static void SetMainPage()
        {
            if (!Settings.IsLoggedIn)
            {
                Current.MainPage = new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = (Color)Current.Resources["Primary"],
                    BarTextColor = Color.White
                };
            }
            else
            {
                GoToMainPage();
            }
        }

        public static void GoToMainPage()
        {
            var menuPage = new MenuPage();
            NavigationPage = new NavigationPage(new HomePage());
            RootPage = new RootPage();
            RootPage.Master = menuPage;
            RootPage.Detail = NavigationPage;
            Current.MainPage = RootPage;
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
