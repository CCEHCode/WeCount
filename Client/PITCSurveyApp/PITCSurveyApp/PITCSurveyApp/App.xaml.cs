using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using PITCSurveyApp.Helpers;
using PITCSurveyApp.Views;

namespace PITCSurveyApp
{
	public partial class App : Application
	{
        public static string AzureMobileAppUrl = "https://pitcsurveyapi.azurewebsites.net";
        public static IDictionary<string, string> LoginParameters => null;

        public App ()
		{
			InitializeComponent();

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
            Current.MainPage = new MainPage();
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
