using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PITCSurveyApp.Helpers;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
		}

#if !WINDOWS_UWP
        protected override void OnAppearing()
        {
            // Windows does not fire the event after returning from the
            // login Web view modal; instead it changes the main page
            // directly after returning from login. For iOS and Android,
            // we change the main page here to avoid an internal state
            // exception that occurs when we try to change the main page
            // before the login modal has closed.
            if (Settings.IsLoggedIn)
            {
                App.GoToMainPage();
            }
        }
#endif
    }
}
