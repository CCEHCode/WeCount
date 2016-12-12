using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Input;
using Xamarin.Forms;

using PITCSurveyApp.Extensions;
using PITCSurveyApp.Views;

namespace PITCSurveyApp.ViewModels
{
    class MenuPageViewModel
    {
        public ICommand GoHomeCommand { get; set; }
        public ICommand GoSettingsCommand { get; set; }

        public MenuPageViewModel()
        {
            GoHomeCommand = new Command(GoHome);
            GoSettingsCommand = new Command(GoSettings);
        }

        void GoHome(object obj)
        {
            App.NavigationPage.Navigation.PopToRootAsync();
            App.MenuIsPresented = false;
        }

        void GoSettings(object obj)
        {
            App.NavigationPage.Navigation.PushAsync(new SettingsPage());
            App.MenuIsPresented = false;
        }
    }
}
