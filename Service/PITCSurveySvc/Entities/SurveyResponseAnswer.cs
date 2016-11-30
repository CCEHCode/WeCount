using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PITCSurveySvc.Entities
{
    public class SurveyResponseAnswer
    {
		[Key]
		public int ID { get; set; }

		[Required]
		[ForeignKey("SurveyResponse_ID")]
		public SurveyResponse SurveyResponse { get; set; }
		public int SurveyResponse_ID { get; set; }

		[Required]
		[ForeignKey("Question_ID")]
		public Question Question { get; set; }
		public int Question_ID { get; set; }

		[Required]
		[ForeignKey("AnswerChoice_ID")]
		public AnswerChoice AnswerChoice { get; set; }
		public int AnswerChoice_ID { get; set; }
	}
}
