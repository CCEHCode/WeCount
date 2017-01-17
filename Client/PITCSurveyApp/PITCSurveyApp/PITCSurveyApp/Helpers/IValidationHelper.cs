namespace PITCSurveyApp.Helpers
{
    interface IValidationHelper
    {
		bool IsValidPhone(string phoneNumber);

		bool IsValidEmail(string emailAddress);
	}
}
