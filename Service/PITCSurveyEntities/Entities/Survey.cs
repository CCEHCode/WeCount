using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PITCSurveyEntities.Entities
{
	public class Survey
	{
		[Key]
		public int ID { get; set; }

		public bool Active { get; set; }

		public String Name { get; set; }

		public int SurveyYear { get; set; }

		public String Description { get; set; }

		public String IntroText { get; set; }

		public virtual IList<SurveyQuestion> SurveyQuestions { get; set; }
	}
}