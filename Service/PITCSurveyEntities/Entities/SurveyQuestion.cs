using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PITCSurveyEntities.Entities
{
	public class SurveyQuestion
	{
		[Key]
		public int ID { get; set; }

		[Required]
		[ForeignKey("Survey_ID")]
		public virtual Survey Survey { get; set; }
		public int Survey_ID { get; set; }

		[Required]
		[ForeignKey("Question_ID")]
		public virtual Question Question { get; set; }
		public int Question_ID { get; set; }

		public String QuestionNum { get; set; }

		public virtual IList<SurveyAnswerChoice> AnswerChoices { get; set; }

		[ForeignKey("DependentQuestion_ID")]
		public virtual Question DependentQuestion { get; set; }
		public int? DependentQuestion_ID { get; set; }

		public virtual IList<AnswerChoice> DependentQuestionAnswers { get; set; }
	}
}