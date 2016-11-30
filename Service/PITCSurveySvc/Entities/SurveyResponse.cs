using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;

namespace PITCSurveySvc.Entities
{
	public class SurveyResponse
	{
		[Key]
		public int ID { get; set; }

		[Required]
		[ForeignKey("Survey_ID")]
		public Survey Survey { get; set; }
		public int Survey_ID { get; set; }

		[Required]
		[ForeignKey("Volunteer_ID")]
		public Volunteer Volunteer { get; set; }
		public int Volunteer_ID { get; set; }

		public String Interviewee { get; set; }
		public DateTimeOffset InterviewStarted { get; set; }
		public DateTimeOffset InterviewCompleted { get; set; }

		public DbGeometry GPSLocation { get; set; }

		public String NearestAddress { get; set; }
		public String LocationNotes { get; set; }

		public virtual IList<SurveyResponseAnswer> Answers { get; set; }

	}
}
