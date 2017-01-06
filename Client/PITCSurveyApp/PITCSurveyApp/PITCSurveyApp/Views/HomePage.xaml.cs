using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using PITCSurveyApp.ViewModels;

namespace PITCSurveyApp.Views
{
	public partial class HomePage : ContentPage
	{
		public HomePage ()
		{
            BindingContext = new HomePageViewModel();
            InitializeComponent ();
		}

	    protected override async void OnAppearing()
	    {
            // We're loading the survey here to avoid async contructors in SurveyViewModel
	        await App.SurveyVM.GetSurvey();

            // Since we're loading this only once on app start it's acceptable to do this
            // here in code instead of using Data Binding in XAML
	        LblSurveyVersionCloud.Text = App.SurveyVM.SurveyVersion;
	        LblSurveyQuestionsCount.Text = App.SurveyVM.SurveyQuestionsCount.ToString();
	    }

    }
}
