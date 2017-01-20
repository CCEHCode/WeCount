using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class MySurveysViewModel : BaseViewModel
    {
        public const int UploadDeleteDelayMilliseconds = 3000;

        private static readonly TimeSpan s_maxAge = TimeSpan.FromDays(2);

        private readonly bool _isLoadOnly;
        private ObservableCollection<MySurveysItemViewModel> _surveys;
        private MySurveysItemViewModel _selectedItem;

        /// <summary>
        /// Instantiates the view model with a flag that signals if the
        /// page is specifically used for loading surveys.
        /// </summary>
        /// <param name="isLoadOnly">
        /// Instantiate with <code>true</code> if the page is intended to load the selected survey immediately.
        /// </param>
        public MySurveysViewModel(bool isLoadOnly)
        {
            _isLoadOnly = isLoadOnly;
            UploadSelectedCommand = new Command(UploadSelected, () => SelectedItem != null);
            UploadAllCommand = new Command(UploadAll, () => Surveys?.Count > 0);
        }

        public Command UploadSelectedCommand { get; }

        public Command UploadAllCommand { get; }

        public bool IsNotLoadOnly => !_isLoadOnly;

        public ObservableCollection<MySurveysItemViewModel> Surveys
        {
            get { return _surveys; }
            private set
            {
                // Unsubscribe from the CollectionChanged event on the previous collection
                if (_surveys != null)
                {
                    _surveys.CollectionChanged -= OnCollectionChanged;
                }

                SetProperty(ref _surveys, value);
                UploadAllCommand.ChangeCanExecute();

                // Subscribe to the CollectionChanged event on the current collection
                if (_surveys != null)
                {
                    _surveys.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        public MySurveysItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                UploadSelectedCommand.ChangeCanExecute();
                if (_isLoadOnly)
                {
                    EditSelectedItem();
                }
            }
        }

        public async Task RefreshAsync()
        {
            Surveys?.Clear();
            try
            {
                // Get all the files that match *.survey.json
                var fileHelper = new FileHelper();
                var files = await fileHelper.GetFilesAsync();
                var surveyFiles = files.Where(f => f.EndsWith(".survey.json"));
                var managers = new List<MySurveysItemViewModel>();
                foreach (var surveyFile in surveyFiles)
                {
                    // Clean up surveys older than maximum age.
                    var lastModified = await fileHelper.LastModifiedAsync(surveyFile);
                    if (DateTime.Now - lastModified > s_maxAge)
                    {
                        await fileHelper.DeleteAsync(surveyFile);
                    }
                    else
                    {
                        var manager = new MySurveysItemViewModel(surveyFile);
                        manager.Deleted += ResponseDeleted;
                        await manager.LoadAsync();
                        managers.Add(manager);
                    }
                }

                // Sort the surveys from most recently saved to least recently saved 
                managers.Sort((x, y) => -CompareDateTime(x.LastModified, y.LastModified));
                Surveys = new ObservableCollection<MySurveysItemViewModel>(managers);
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("MySurveysRefreshAsyncFailed", ex);
                await App.DisplayAlertAsync(
                    "My Surveys Refresh Failed", 
                    "Failed to refresh stored surveys, please try again.",
                    "OK");
            }
        }

        private async void EditSelectedItem()
        {
            if (_selectedItem != null)
            {
                DependencyService.Get<IMetricsManagerService>().TrackEvent("MySurveysLoadOnlyEdit");
                await _selectedItem.EditAsync();
                SelectedItem = null;
            }
        }

        private async void UploadSelected()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("MySurveysUploadSelected");

            var selectedItem = SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            try
            {
                await selectedItem.UploadAndDeleteAsync();
            }
            catch
            {
                await App.DisplayAlertAsync(
                    "Upload Failed",
                    "Failed to upload survey. Please try again.",
                    "OK");
            }
        }

        private async void UploadAll()
        {
            DependencyService.Get<IMetricsManagerService>().TrackEvent("MySurveysUploadAll");
            var surveys = new List<MySurveysItemViewModel>(Surveys);
            foreach (var item in surveys)
            {
                try
                {
                    await item.UploadAsync(false);
                    await item.DeleteAsync();
                }
                catch
                {
                    await App.DisplayAlertAsync(
                        "Upload Failed",
                        "At least one survey upload failed. Please try again.",
                        "OK");
                }
            }

            await Task.Delay(UploadDeleteDelayMilliseconds);
            await RefreshAsync();
        }

        private int CompareDateTime(DateTime? x, DateTime? y)
        {
            if (x == y)
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            return x.Value.CompareTo(y.Value);
        }

        private void ResponseDeleted(object sender, EventArgs args)
        {
            var item = (MySurveysItemViewModel)sender;
            if (item == SelectedItem)
            {
                SelectedItem = null;    
            }

            Surveys.Remove(item);
        }

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UploadAllCommand.ChangeCanExecute();
        }
    }
}
