using PITCSurveyApp.ViewModels;
using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
    public partial class SurveyAnswerItemAndroidView : ContentView
    {
        public SurveyAnswerItemAndroidView (SurveyAnswerItemViewModel viewModel)
        {
            InitializeComponent ();

            BindingContext = viewModel;
        }
    }
}
