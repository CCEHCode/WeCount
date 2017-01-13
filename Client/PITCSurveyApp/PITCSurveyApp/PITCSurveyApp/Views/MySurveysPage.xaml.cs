using PITCSurveyApp.ViewModels;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
	public partial class MySurveysPage : ContentPage
	{
	    private readonly MySurveysViewModel _viewModel;

	    public MySurveysPage()
	        : this(false)
	    {
	    }

        public MySurveysPage(bool loadOnly)
        {
			InitializeComponent();
		    _viewModel = new MySurveysViewModel(loadOnly);
		    BindingContext = _viewModel;
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.RefreshAsync();
        }
    }
}
