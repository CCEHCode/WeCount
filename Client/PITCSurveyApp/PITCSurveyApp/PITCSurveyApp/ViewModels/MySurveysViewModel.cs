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
        private readonly INavigation _navigation;
        private ObservableCollection<MySurveysItemViewModel> _surveys;

        public MySurveysViewModel(INavigation navigation)
        {
            _navigation = navigation;
            IsBusy = true;
            Init();
        }

        public ObservableCollection<MySurveysItemViewModel> Surveys
        {
            get { return _surveys; }
            private set { SetProperty(ref _surveys, value); }
        }

        private async void Init()
        {
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

            IsBusy = false;
            Surveys = new ObservableCollection<MySurveysItemViewModel>(managers);
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
            Surveys.Remove((MySurveysItemViewModel) sender);
        }
    }
}
