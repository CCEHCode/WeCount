using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PITCSurveyEntities.Entities
{
    public class SurveyResponseAnswer
    {
		[Key]
		public int ID { get; set; }

		[ForeignKey("SurveyResponse_ID")]
		public virtual SurveyResponse SurveyResponse { get; set; }
		[Required]
		public int SurveyResponse_ID { get; set; }

		[ForeignKey("Question_ID")]
		public virtual Question Question { get; set; }
		[Required]
		public int Question_ID { get; set; }

		[ForeignKey("AnswerChoice_ID")]
		public virtual AnswerChoice AnswerChoice { get; set; }
		[Required]
		public int AnswerChoice_ID { get; set; }

		public string AdditionalAnswerData { get; set; }
	}
}
