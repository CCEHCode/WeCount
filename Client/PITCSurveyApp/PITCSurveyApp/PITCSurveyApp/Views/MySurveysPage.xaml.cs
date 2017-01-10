using PITCSurveyApp.ViewModels;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
	public partial class MySurveysPage : ContentPage
	{
	    private readonly MySurveysViewModel _viewModel;

		public MySurveysPage ()
		{
			InitializeComponent ();

		    _viewModel = new MySurveysViewModel();
		    BindingContext = _viewModel;
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.RefreshAsync();
        }
    }
}
