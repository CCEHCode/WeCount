using PITCSurveyApp.ViewModels;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
	public partial class MySurveysPage : ContentPage
	{
		public MySurveysPage ()
		{
			InitializeComponent ();

		    BindingContext = new MySurveysViewModel(Navigation);
		}
	}
}
