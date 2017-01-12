using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class MySurveysViewModel : BaseViewModel
    {
        private ObservableCollection<MySurveysItemViewModel> _surveys;
        private MySurveysItemViewModel _selectedItem;

        public MySurveysViewModel()
        {
            UploadSelectedCommand = new Command(UploadSelected, () => SelectedItem != null);
            UploadAllCommand = new Command(UploadAll, () => Surveys?.Count > 0);
        }

        public Command UploadSelectedCommand { get; }

        public Command UploadAllCommand { get; }

        public ObservableCollection<MySurveysItemViewModel> Surveys
        {
            get { return _surveys; }
            private set
            {
                if (_surveys != null)
                {
                    _surveys.CollectionChanged -= OnCollectionChanged;
                }

                SetProperty(ref _surveys, value);
                UploadAllCommand.ChangeCanExecute();

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
            }
        }

        public async Task RefreshAsync()
        {
            Surveys?.Clear();
            var fileHelper = new FileHelper();
            var files = await fileHelper.GetFilesAsync();
            var surveyFiles = files.Where(f => f.EndsWith(".survey.json"));
            var managers = new List<MySurveysItemViewModel>();
            foreach (var surveyFile in surveyFiles)
            {
                var manager = new MySurveysItemViewModel(surveyFile);
                manager.Deleted += ResponseDeleted;
                await manager.LoadAsync();
                managers.Add(manager);
            }

            managers.Sort((x, y) => -CompareDateTime(x.LastModified, y.LastModified));
            Surveys = new ObservableCollection<MySurveysItemViewModel>(managers);
        }


        private async void UploadSelected()
        {
            var selectedItem = SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            try
            {
                await selectedItem.UploadAsync();
            }
            catch
            {
                App.DisplayAlert(
                    "Upload Failed",
                    "Failed to upload survey. Please try again.",
                    "OK");
            }
        }

        private async void UploadAll()
        {
            var uploadFailed = false;
            var tasks = new List<Task>();
            foreach (var item in Surveys)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await item.UploadAsync();
                    }
                    catch
                    {
                        uploadFailed = true;
                    }
                }));
            }

            await Task.WhenAll(tasks);
            if (uploadFailed)
            {
                App.DisplayAlert(
                    "Upload Failed",
                    "At least one survey upload failed. Please try again.",
                    "OK");
            }
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
