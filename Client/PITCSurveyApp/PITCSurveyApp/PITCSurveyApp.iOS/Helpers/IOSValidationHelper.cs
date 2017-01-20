using com.google.i18n.phonenumbers;
using PITCSurveyApp.Helpers;
using System;
using System.Net.Mail;
using Xamarin.Forms;

[assembly: Dependency(typeof(PITCSurveyApp.iOS.Helpers.IOSValidationHelper))]

namespace PITCSurveyApp.iOS.Helpers
{
	class IOSValidationHelper : IValidationHelper
	{
		public bool IsValidEmail(string emailAddress)
		{
			try
			{

				// This is RFC-5322 compliant.
				var address = new MailAddress(emailAddress);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool IsValidPhone(string phoneNumber)
		{
			var util = PhoneNumberUtil.getInstance();

			try
			{
				var parsedPhoneNumber = util.parse(phoneNumber, "US");
				return util.isValidNumber(parsedPhoneNumber);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}