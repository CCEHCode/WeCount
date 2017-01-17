using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.ViewModels
{
    class ContactInfoViewModel : BaseViewModel
    {
		private string _Note = "Loading...";
		private string _ContactInfo = "Loading...";

		public ContactInfoViewModel()
		{
			IsBusy = true;

			UpdateContactInfo();
		}

		public string Note
		{
			get { return _Note; }
			set { SetProperty(ref _Note, value); }
		}

		public string ContactInfo
		{
			get { return _ContactInfo; }
			set { SetProperty(ref _ContactInfo, value); }
		}

		private async void UpdateContactInfo()
		{
			try
			{
				var Model = await SurveyCloudService.GetContactInfoAsync(null);    // TODO: Put SurveyId here

				this.Note = Model.Notes;

				this.ContactInfo = string.Join("\n", Model.Contacts.Select(c => $"{c.Name}: {c.Phone}").ToArray());
			}
			catch (Exception)
			{
				this.Note = "Error";
				this.ContactInfo = null;
			}

			IsBusy = false;
		}
	}
}
