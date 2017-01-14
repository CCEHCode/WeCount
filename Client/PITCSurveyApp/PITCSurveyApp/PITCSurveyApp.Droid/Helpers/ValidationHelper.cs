using com.google.i18n.phonenumbers;
using PITCSurveyApp.Helpers;
using System;
using System.Net.Mail;
using Xamarin.Forms;

[assembly: Dependency(typeof(PITCSurveyApp.Droid.Helpers.ValidationHelper))]

namespace PITCSurveyApp.Droid.Helpers
{
	class ValidationHelper : IValidationHelper
	{
		public bool IsValidEmail(string EmailAddress)
		{
			try
			{
				// This is RFC-5322 compliant.
				MailAddress Addr = new MailAddress(EmailAddress);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool IsValidPhone(string PhoneNumber)
		{
			var Util = PhoneNumberUtil.getInstance();

			try
			{
				var PhNum = Util.parse(PhoneNumber, "US");
				return Util.isValidNumber(PhNum);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}