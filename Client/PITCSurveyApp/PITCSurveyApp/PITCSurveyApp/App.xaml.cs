using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;
using PITCSurveyApp.Views;
using PITCSurveyLib;
using PITCSurveyLib.Models;

namespace PITCSurveyApp
{
    public partial class App : Application
	{
        public static SurveyModel LatestSurvey { get; set; }

	    private static IAuthenticate s_authenticator;

        public static void Init(IAuthenticate authenticator)
        {
            s_authenticator = authenticator;
        }

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

        public static async void GoToMainPage()
        {
            var menuPage = new MenuPage();
            NavigationPage = new NavigationPage(new HomePage());
            RootPage = new RootPage();
            RootPage.Master = menuPage;
            RootPage.Detail = NavigationPage;
            Current.MainPage = RootPage;
        }

	    public static async Task LoginAsync(MobileServiceAuthenticationProvider provider)
	    {
	        try
	        {
	            DependencyService.Get<IMetricsManagerService>().TrackEvent("UserLogin");
	            var user = await s_authenticator.AuthenticateAsync(provider);
	            Settings.AuthToken = user?.MobileServiceAuthenticationToken;
	            APIHelper.AuthToken = user?.MobileServiceAuthenticationToken;
	        }
	        catch (Exception ex)
	        {
                DependencyService.Get<IMetricsManagerService>().TrackException("UserLoginFailed", ex);
            }
        }

	    public static async Task LogoutAsync()
	    {
	        try
	        {
	            DependencyService.Get<IMetricsManagerService>().TrackEvent("UserLogout");
	            await s_authenticator.LogoutAsync();
	            Settings.AuthToken = null;
	            APIHelper.AuthToken = null;
	        }
	        catch (Exception ex)
	        {
                DependencyService.Get<IMetricsManagerService>().TrackException("UserLogoutFailed", ex);
            }
        }

	    public static void DisplayAlert(string title, string message, string cancel)
	    {
	        RootPage.DisplayAlert(title, message, cancel);
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
