namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// An interface for required user input validations.
    /// </summary>
    interface IValidationHelper
    {
        /// <summary>
        /// Checks if the given text is a valid phone number. 
        /// </summary>
        /// <param name="phoneNumber">The phone number text.</param>
        /// <returns>
        /// <code>true</code> if the phone number is valid, otherwise <code>false</code>.
        /// </returns>
		bool IsValidPhone(string phoneNumber);

        /// <summary>
        /// Checks if the given text is a valid email address.
        /// </summary>
        /// <param name="emailAddress">The email address text.</param>
        /// <returns>
        /// <code>true</code> if the email address is valid, otherwise <code>false</code>.
        /// </returns>
		bool IsValidEmail(string emailAddress);
	}
}
