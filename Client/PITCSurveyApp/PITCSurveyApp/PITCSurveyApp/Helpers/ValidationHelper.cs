using System;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    class ValidationHelper: IValidationHelper
    {
		private readonly IValidationHelper _validationHelper = DependencyService.Get<IValidationHelper>();

		public bool IsValidPhone(string phoneNumber)
		{
			try
			{
				return _validationHelper.IsValidPhone(phoneNumber);
			}
			catch (Exception ex)
			{
			    DependencyService.Get<IMetricsManagerService>().TrackException("PhoneValidationFailure", ex);
				return false;
			}
		}

		public bool IsValidEmail(string emailAddress)
		{
			try
			{
				return _validationHelper.IsValidEmail(emailAddress);
			}
			catch (Exception ex)
			{
                DependencyService.Get<IMetricsManagerService>().TrackException("EmailValidationFailure", ex);
                return false;
			}
		}
    }
}
