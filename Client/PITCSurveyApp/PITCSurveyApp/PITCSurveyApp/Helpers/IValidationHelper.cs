using System;
using System.Collections.Generic;
using System.Text;

namespace PITCSurveyApp.Helpers
{
    interface IValidationHelper
    {
		bool IsValidPhone(string PhoneNumber);

		bool IsValidEmail(string EmailAddress);
	}
}
