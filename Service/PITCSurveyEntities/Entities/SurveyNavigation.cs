using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PITCSurveyEntities.Entities
{
	public class SurveyNavigation
	{
		[Key]
		public int ID { get; set; }

		//[Required]
		//[ForeignKey("Survey_ID")]
		//public Survey Survey { get; set; }
		//public int Survey_ID { get; set; }

		[Required]
		[ForeignKey("SurveyQuestion_ID")]
		public virtual SurveyQuestion SurveyQuestion { get; set; }
		public int SurveyQuestion_ID { get; set; }

		[Required]
		[ForeignKey("AnswerChoice_ID")]
		public virtual AnswerChoice AnswerChoice { get; set; }
		public int AnswerChoice_ID { get; set; }

		//[Required]
		[ForeignKey("NextSurveyQuestion_ID")]
		public virtual SurveyQuestion NextSurveyQuestion { get; set; }
		public int? NextSurveyQuestion_ID { get; set; }
	}
}