using PITCSurveyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;

namespace PITCSurveyEntities.Entities
{
	public class SurveyResponse
	{
		[Key]
		public int ID { get; set; }

		public Guid ResponseIdentifier { get; set; }

		[ForeignKey("Survey_ID")]
		public virtual Survey Survey { get; set; }
		[Required]
		public int Survey_ID { get; set; }

		public int Survey_Version { get; set; }

		public Guid DeviceId { get; set; }

		[ForeignKey("Volunteer_ID")]
		public virtual Volunteer Volunteer { get; set; }
		[Required]
		public int Volunteer_ID { get; set; }

		public DateTimeOffset InterviewStarted { get; set; }
		public DateTimeOffset InterviewCompleted { get; set; }

		public DbGeography GPSLocation { get; set; }

		public String LocationNotes { get; set; }

		public Address NearestAddress { get; set; } = new Address();

		public virtual IList<SurveyResponseAnswer> Answers { get; set; } = new List<SurveyResponseAnswer>();

	}
}
