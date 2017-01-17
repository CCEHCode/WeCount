using System;
using System.Text.RegularExpressions;
using PhoneNumbers;
using PITCSurveyApp.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(PITCSurveyApp.UWP.Helpers.ValidationHelper))]

namespace PITCSurveyApp.UWP.Helpers
{
	class ValidationHelper : IValidationHelper
	{
		private static readonly Regex s_emailRegex = new Regex(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);

		public bool IsValidEmail(string emailAddress)
		{
			// UWP is the only platform that *doesn't* support System.Net.Mail(!)
			// TODO: Just use a RegEx here? That can be problematic, as there are ways to validate an address confirms to the RFC,
			// but also mail systems that allow non-compliant addresses, which are valid for those systems.
			// Good writeup here: https://github.com/tallesl/net-emailAddress
			// That one also references some other good ones: 
			// http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx/
			// https://www.troyhunt.com/dont-trust-net-web-forms-email-regex/
			// All are great examples of what happens when the theory/spec crashes into the real world usage.

			try
			{
				// In the meantime, we'll use at least a not completely sucky solution.
				return s_emailRegex.IsMatch(emailAddress);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool IsValidPhone(string phoneNumber)
		{
			var util = PhoneNumberUtil.GetInstance();

			try
			{
				var parsedPhoneNumber = util.Parse(phoneNumber, "US");
				return util.IsValidNumber(parsedPhoneNumber);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}