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
			try
			{
				return _ValidationHelper.IsValidPhone(PhoneNumber);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError($"Validation for PhoneNumber failed: {ex.Message}");
				return false;
			}
		}

		public bool IsValidEmail(string EmailAddress)
		{
			try
			{
				return _ValidationHelper.IsValidEmail(EmailAddress);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError($"Validation for EmailAddress failed: {ex.Message}");
				return false;
			}
		}
    }
}
