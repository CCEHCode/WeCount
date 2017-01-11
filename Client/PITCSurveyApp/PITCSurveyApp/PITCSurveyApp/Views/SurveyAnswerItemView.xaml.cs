using PITCSurveyApp.ViewModels;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
	public partial class SurveyAnswerItemView : ContentView
	{
		public SurveyAnswerItemView (SurveyAnswerItemViewModel viewModel)
		{
			InitializeComponent ();

		    BindingContext = viewModel;
		}
	}
}
