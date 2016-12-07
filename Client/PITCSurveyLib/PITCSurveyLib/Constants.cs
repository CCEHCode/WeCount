using System;

namespace PITCSurveyLib
{
	/// <summary>
	/// Specifies the expected format for AdditionalAnswerData on AnswerChoices with a free-form or additional data field.
	/// </summary>
	public enum AnswerFormat : int
	{
		None = 0,
		String = 1,
		Int = 2,
		Date = 3
	}

	[Obsolete("This no longer needs to be passed, as the auth mechanism returns it.")]
	public enum AuthMethod: int
	{
		Google = 1,
		Facebook = 2,
		Microsoft = 3
	}
}
