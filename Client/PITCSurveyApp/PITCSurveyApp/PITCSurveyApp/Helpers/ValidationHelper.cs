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
				// Used for testing in Droid & iOS. Have to block out for UWP - somehow, again, the Windows platform is the only one that doesn't support much of .NET!
				//System.Diagnostics.Trace.TraceError($"Validation for PhoneNumber failed: {ex.Message}");
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
				// Used for testing in Droid & iOS. Have to block out for UWP - somehow, again, the Windows platform is the only one that doesn't support much of .NET!
				//System.Diagnostics.Trace.TraceError($"Validation for EmailAddress failed: {ex.Message}");
				return false;
			}
		}
    }
}
