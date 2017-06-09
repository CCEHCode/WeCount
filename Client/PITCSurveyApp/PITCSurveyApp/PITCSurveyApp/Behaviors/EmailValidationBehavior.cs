using PITCSurveyApp.Helpers;
using Xamarin.Forms;

namespace PITCSurveyApp.Behaviors
{
    /// <summary>
    /// Email validation behavior for profile text entry.
    /// </summary>
    class EmailValidationBehavior : Behavior<Entry>
    {
        private static readonly IValidationHelper s_validationHelper = new ValidationHelper();

        /// <summary>
        /// Subscribe to text changed events on text entry component.
        /// </summary>
        /// <param name="bindable">The text entry component.</param>
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;

            base.OnAttachedTo(bindable);
        }

        /// <summary>
        /// Unsubscribe from text changed events on text entry component.
        /// </summary>
        /// <param name="bindable">The text entry component.</param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;

            base.OnDetachingFrom(bindable);
        }

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            var isValid = s_validationHelper.IsValidEmail(e.NewTextValue);

            ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }
    }
}
