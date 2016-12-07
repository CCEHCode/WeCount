using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PITCSurveyEntities.Entities
{
	public class Question
	{
		[Key]
		public int ID { get; set; }

		public String Key { get; set; }

		public String QuestionText { get; set; }
		public String ClarificationText { get; set; }

		public bool AllowMultipleAnswers { get; set; }

		public virtual IList<AnswerChoice> AnswerChoices { get; set; }
	}
}