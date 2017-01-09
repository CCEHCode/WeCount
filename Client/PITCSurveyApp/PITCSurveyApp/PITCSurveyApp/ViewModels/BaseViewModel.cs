using PITCSurveyApp.Helpers;

namespace PITCSurveyApp.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public bool IsNotBusy => !_isBusy;
    }
}
