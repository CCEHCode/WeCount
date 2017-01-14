using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    class ValidationHelper: IValidationHelper
    {
		private readonly IValidationHelper _ValidationHelper = DependencyService.Get<IValidationHelper>();

		public bool IsValidPhone(string PhoneNumber)
		{
			return _ValidationHelper.IsValidPhone(PhoneNumber);
		}

		public bool IsValidEmail(string EmailAddress)
		{
			return _ValidationHelper.IsValidEmail(EmailAddress);
		}
    }
}
