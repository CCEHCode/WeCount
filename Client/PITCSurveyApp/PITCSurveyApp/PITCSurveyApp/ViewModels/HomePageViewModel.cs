using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Lib.Services;
using PITCSurveyApp.Lib.ViewModels;
using PITCSurveyLib.Models;

namespace PITCSurveyApp.ViewModels
{
    class HomePageViewModel
    {
        private SurveyViewModel vm { get; set; }

        public HomePageViewModel()
        {
            App.SurveyVM = new SurveyViewModel();      

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
            get { return App.SurveyVM.SurveyVersionCloud; }
        }

        public string SurveyVersionLocal
        {
            get { return App.SurveyVM.SurveyVersionLocal; }
        }

        public int SurveyQuestionsCount
        {
            get { return App.SurveyVM.SurveyQuestionsCount; }
        }
    }
}
