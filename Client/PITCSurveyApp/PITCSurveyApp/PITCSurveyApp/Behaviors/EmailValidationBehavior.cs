using PITCSurveyApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PITCSurveyApp.Behaviors
{
    class EmailValidationBehavior : Behavior<Entry>
    {
		static IValidationHelper _validationHelper = new ValidationHelper();

		protected override void OnAttachedTo(Entry bindable)
		{
			bindable.TextChanged += HandleTextChanged;

			base.OnAttachedTo(bindable);
		}

		protected override void OnDetachingFrom(Entry bindable)
		{
			bindable.TextChanged -= HandleTextChanged;

			base.OnDetachingFrom(bindable);
		}

		private void HandleTextChanged(object sender, TextChangedEventArgs e)
		{
			bool IsValid = _validationHelper.IsValidEmail(e.NewTextValue);

			((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
		}
	}
}
