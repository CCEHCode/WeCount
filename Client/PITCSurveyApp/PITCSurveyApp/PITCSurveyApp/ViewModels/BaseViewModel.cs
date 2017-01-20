using PITCSurveyApp.Helpers;

namespace PITCSurveyApp.ViewModels
{
    /// <summary>
    /// Base class for view models including <see cref="System.ComponentModel.INotifyPropertyChanged"/>
    /// and bindable <see cref="IsBusy"/> and <see cref="IsNotBusy"/> properties. 
    /// </summary>
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;

        /// <summary>
        /// If <code>true</code>, indicates the view is busy.
        /// </summary>
        /// <remarks>
        /// This is a useful property to bind an activity indicator visibility to.
        /// </remarks>
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

        /// <summary>
        /// If <code>true</code>, signals the activity is not busy.
        /// </summary>
        /// <remarks>
        /// This is a useful property to bind a button enabled property to.
        /// </remarks>
        public bool IsNotBusy => !_isBusy;
    }
}
