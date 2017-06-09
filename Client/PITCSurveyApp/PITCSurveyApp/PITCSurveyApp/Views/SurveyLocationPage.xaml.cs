using PITCSurveyApp.Models;
using PITCSurveyApp.ViewModels;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
    public partial class SurveyLocationPage : ContentPage
    {
        private readonly SurveyLocationViewModel _viewModel;

        public SurveyLocationPage ()
            : this(new SurveyLocationViewModel())
        {
        }

        public SurveyLocationPage(UploadedItem<SurveyResponseModel> response, bool updateLocation)
            : this(new SurveyLocationViewModel(response, updateLocation))
        {
        }

        private SurveyLocationPage(SurveyLocationViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            // Save the survey each time the location page disappears,
            // but only if the location page was created from the toolbar.
            // This prevents the creation of an empty survey every time
            // a new survey is started and immediately canceled.
            if (_viewModel.IsUpdateLocation)
            {
                await _viewModel.SaveAsync();
            }
        }
    }
}
