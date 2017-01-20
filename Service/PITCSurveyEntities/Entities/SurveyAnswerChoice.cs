using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITCSurveyEntities.Entities
{
	public class SurveyAnswerChoice
	{
		[Key]
		public int ID { get; set; }

		[ForeignKey("SurveyQuestion_ID")]
		public virtual SurveyQuestion SurveyQuestion { get; set; }
		[Required]
		public int SurveyQuestion_ID { get; set; }

		[ForeignKey("AnswerChoice_ID")]
		public virtual AnswerChoice AnswerChoice { get; set; }
		[Required]
		public int AnswerChoice_ID { get; set; }

		public String AnswerChoiceNum { get; set; }

		[ForeignKey("NextSurveyQuestion_ID")]
		public virtual SurveyQuestion NextSurveyQuestion { get; set; }
		public int? NextSurveyQuestion_ID { get; set; }

		public bool EndSurvey { get; set; }
	}
}
