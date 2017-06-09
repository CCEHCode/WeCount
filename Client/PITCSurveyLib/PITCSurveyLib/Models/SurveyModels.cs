using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PITCSurveyLib.Models
{
    /// <summary>
    /// Client-side model for the survey form to be displayed for each interviewee.
    /// </summary>
    public class SurveyModel
    {
        // TODO: Will string replacements be handled client-side, or server-side?
        
        public int SurveyID { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }
        public String IntroText { get; set; }

        public int Version { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public IList<SurveyQuestionModel> Questions { get; set; } = new List<SurveyQuestionModel>();
    }

    /// <summary>
    /// Client-side model for individual survey questions to be displayed for each interviewee.
    /// </summary>
    public class SurveyQuestionModel
    {
        public int QuestionID { get; set; }
        public String QuestionNum { get; set; }
        public String QuestionText { get; set; }
        public String QuestionHelpText { get; set; }
        public bool AllowMultipleAnswers {get; set; }

        public WellKnownQuestion WellKnownQuestion { get; set; }

        public IList<SurveyQuestionAnswerChoiceModel> AnswerChoices { get; set; } = new List<SurveyQuestionAnswerChoiceModel>();
    }

    /// <summary>
    /// Client-side model for possible answer choices for each question to be displayed for each interviewee.
    /// </summary>
    public class SurveyQuestionAnswerChoiceModel
    {
        public int AnswerChoiceID { get; set; }                            // Database ID
        public String AnswerChoiceNum { get; set; }                        // Displayed number (accounts for sequences like 5, 6, 6a, 7
        public String AnswerChoiceText { get; set; }
        public AnswerFormat AdditionalAnswerDataFormat { get; set; }    // Format of additional answer data: none, int, string, etc.
        public int? NextQuestionID { get; set; }                        // ID of next Question to display, if this AnswerChoice is selected.
                                                                        // If null, default to next question in sequence.
        public bool EndSurvey { get; set; }                             // If selected, this answer choice ends the survey.

        [Obsolete("Use EndSurvey bool property instead.")]              // Deprecating this, to avoid use of "magic numbers".
        [JsonIgnore()]
        public const int END_SURVEY = -1;                                // If NextQuestionID is this value, then survey is complete.
    }

}
