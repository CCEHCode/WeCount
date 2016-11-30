using System;
using System.Collections.Generic;

namespace PITCSurveyLib.Models
{
	/// <summary>
	/// Client-side response model for the survey response from a given interviewee.
	/// Contains only identifiers needed for the responses.
	/// </summary>
	public class SurveyResponseModel
	{
		public int SurveyID { get; set; }

		public String InterviewerID { get; set; }

		// While most questions only allow one answer, this will allow multiple-choice responses.
		public IList<SurveyQuestionResponseModel> QuestionResponses { get; set; } = new List<SurveyQuestionResponseModel>();
	}

	/// <summary>
	/// Client-side response model for individual survey question responses from a given interviewee.
	/// </summary>
	public class SurveyQuestionResponseModel
	{
		public int QuestionID { get; set; }

		public IList<SurveyQuestionAnswerChoiceResponseModel> AnswerChoiceResponses { get; set; } = new List<SurveyQuestionAnswerChoiceResponseModel>();
	}

	/// <summary>
	/// Client-side responsemodel for possible answer choices for each question to be displayed for each interviewee.
	/// </summary>
	public class SurveyQuestionAnswerChoiceResponseModel
	{
		public int QuestionID { get; set; }                             // Database ID
		public int AnswerChoiceID { get; set; }                         // Database ID
		public String AdditionalAnswerData { get; set; }                // Should be formatted / validated according to specified AdditionalAnswerData
	}
}
