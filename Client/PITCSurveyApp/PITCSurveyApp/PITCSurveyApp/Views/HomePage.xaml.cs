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
	}
}
