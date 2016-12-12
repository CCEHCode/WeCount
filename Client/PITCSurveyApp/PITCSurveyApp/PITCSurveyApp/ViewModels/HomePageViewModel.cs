using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using PITCSurveyApp.Helpers;

namespace PITCSurveyApp.ViewModels
{
    class HomePageViewModel
    {
        public string UserFullname { get; set; }

        public string UserGreeting
        {
            get { return "Welcome " + UserFullname; }
        }

        public HomePageViewModel()
        {
            // TO DO: Need to populate this from the authentication service
            UserFullname = "Volunteer";
        }

        public ImageSource BannerImage
        {
            get { return ImageSource.FromFile(CrossHelper.GetOSFullImagePath("ccehlogo.jpg")); }
        }
    }
}
