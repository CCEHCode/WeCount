using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Diagnostics;

namespace PITCSurveyApp.Lib.ViewModel
{
    /// <summary>
    /// ViewModel class used to manage
    /// </summary>
    class SurveyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // We use an ObservableCollection because it has built-in support for
        // CollectionChanged events when we Add or Remove from it.
        // The benefit is we don't have to call OnPropertyChanged each time
        public ObservableCollection<Survey> Surveys { get; set; }

        // Instead of invoking GetSurveys directly, we expose it with a Command.
        // A Command has an interface that knows what method to invoke and has an 
        // optional way of describing if the Command is enabled.
        public Command GetSurveysCommand { get; set; }

        // This will let our view know that our view model is busy so we don't
        // perform duplicate operations
        // (e.g. allowing the user to refresh the data multiple times)
        private bool busy;
        public bool IsBusy
        {
            get { return busy; }
            set
            {
                busy = value;
                OnPropertyChanged();
                //Update the can execute
                GetSurveysCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        /// SurveyViewModel Constructor
        /// </summary>
        public SurveyViewModel()
        {
            // Initialize the Surveys collection
            Surveys = new ObservableCollection<Survey>();

            GetSurveysCommand = new Command(
                async () => await GetSurveys(),
                () => !IsBusy);
        }

        /// <summary>
        /// Load the surveys from local storage and add them to the Surveys ObservableCollection
        /// </summary>
        /// <returns></returns>
        private async Task GetSurveys()
        {
            if (IsBusy)
                return;

            Exception error = null;
            try
            {
                IsBusy = true;

                // TO DO: Load the surveys from JSON files in local storage and add them to
                // the Surveys ObservableCollection

                // Something needs to be awaited here since this is an Async method

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                error = ex;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
