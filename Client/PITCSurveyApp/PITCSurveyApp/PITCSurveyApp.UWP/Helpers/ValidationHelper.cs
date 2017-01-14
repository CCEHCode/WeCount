using PITCSurveyApp.Helpers;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(PITCSurveyApp.UWP.Helpers.ValidationHelper))]

namespace PITCSurveyApp.UWP.Helpers
{
	class ValidationHelper : IValidationHelper
	{
		public bool IsValidEmail(string EmailAddress)
		{
			// UWP is the only platform that *doesn't* support System.Net.Mail(!)
			return true;
		}

		public bool IsValidPhone(string PhoneNumber)
		{
			// UWP doesn't seem to be supported by Google's libphonenumber
			return true;
		}
	}
}