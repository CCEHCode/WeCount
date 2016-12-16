using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using PITCSurveyApp.Helpers;

namespace PITCSurveyApp.ViewModels
{
    class HomePageViewModel
    {
        public HomePageViewModel()
        {
            // TO DO: Need to populate this from the authentication service
            UserFullname = "Volunteer";
        }

        public string UserFullname { get; set; }

        public string UserGreeting
        {
            get { return "Welcome " + UserFullname; }
        }

        public ImageSource BannerImage
        {
            get { return ImageSource.FromFile(CrossHelper.GetOSFullImagePath("ccehlogo.jpg")); }
        }

        public string SurveyVersionCloud
        {
            // TO DO: Get the actual version from the survey in Azure
            get { return "1.0"; }
        }

        public string SurveyVersionLocal
        {
            // TO DO: Get the actual version from the survey store locally
            get { return "1.0"; }
        }
    }
}
