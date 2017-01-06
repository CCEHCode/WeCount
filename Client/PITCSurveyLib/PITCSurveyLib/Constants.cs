using System;

namespace PITCSurveyLib
{
	/// <summary>
	/// Specifies the expected format for AdditionalAnswerData on AnswerChoices with a free-form or additional data field.
	/// </summary>
	public enum AnswerFormat: int
	{
		/// <summary>
		/// This answer choice needs to additional answer data.
		/// </summary>
		None = 0,

		/// <summary>
		/// This answer choice expects additional string data.
		/// </summary>
		String = 1,

		/// <summary>
		/// This answer choice expects additional integer data.
		/// </summary>
		Int = 2,

		/// <summary>
		/// This answer choice expects an additional date value.
		/// </summary>
		Date = 3
	}

	public enum WellKnownQuestion: int
	{
		None = 0,
		NameOrInitials = 1,
		Gender = 2,
		DOB = 3
	}

	[Obsolete("This no longer needs to be passed, as the auth mechanism returns it.")]
	public enum AuthMethod: int
	{
		Google = 1,
		Facebook = 2,
		Microsoft = 3
	}
}
