using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITCSurveyEntities.Entities
{
	public class SurveyQuestion
	{
		[Key]
		public int ID { get; set; }

		[ForeignKey("Survey_ID")]
		public virtual Survey Survey { get; set; }
		[Required]
		public int Survey_ID { get; set; }

		[ForeignKey("Question_ID")]
		public virtual Question Question { get; set; }
		[Required]
		public int Question_ID { get; set; }

		public String QuestionNum { get; set; }

		public virtual IList<SurveyAnswerChoice> AnswerChoices { get; set; }

		[ForeignKey("DependentQuestion_ID")]
		public virtual Question DependentQuestion { get; set; }
		public int? DependentQuestion_ID { get; set; }

		public virtual IList<AnswerChoice> DependentQuestionAnswers { get; set; }
	}
}