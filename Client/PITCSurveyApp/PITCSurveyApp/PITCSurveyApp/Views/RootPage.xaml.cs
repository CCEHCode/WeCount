using Xamarin.Forms;

namespace PITCSurveyApp.Views
{
    public partial class RootPage : MasterDetailPage
    {
        public RootPage ()
        {
            InitializeComponent ();
            MasterBehavior = MasterBehavior.Popover;
        }
    }
}
