using System;
using System.Linq;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class ContactInfoViewModel : BaseViewModel
    {
		private string _note = "Loading...";
		private string _contactInfo = "Loading...";

		public ContactInfoViewModel()
		{
			IsBusy = true;

			UpdateContactInfo();
		}

		public string Note
		{
			get { return _note; }
			set { SetProperty(ref _note, value); }
		}

		public string ContactInfo
		{
			get { return _contactInfo; }
			set { SetProperty(ref _contactInfo, value); }
		}

		private async void UpdateContactInfo()
		{
			try
			{
				var model = await SurveyCloudService.GetContactInfoAsync(1);
                Note = model.Notes;
                ContactInfo = string.Join("\n", model.Contacts.Select(c => $"{c.Name}: {c.Phone}").ToArray());
			}
			catch (Exception ex)
			{
			    DependencyService.Get<IMetricsManagerService>().TrackException("GetContactInfoFailed", ex);
				Note = "Error";
				ContactInfo = null;
			}

			IsBusy = false;
		}
	}
}
