using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PITCSurveyLib;

namespace PITCSurveySvc.Entities
{
	public class AnswerChoice
	{
		[Key]
		public int ID { get; set; }

		public String Key { get; set; }

		public String AnswerText { get; set; }

		public string AdditionalAnswerData { get; set; }

		public AnswerFormat AdditionalAnswerDataFormat { get; set; }
	}
}
