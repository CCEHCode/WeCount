using PITCSurveyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PITCSurveyEntities.Entities
{
	public class Volunteer
    {
		[Key]
		public int ID { get; set; }

		public String FirstName { get; set; }
		public String LastName { get; set; }
		public String Email { get; set; }
		public String MobilePhone { get; set; }
		public String HomePhone { get; set; }
		public Address Address { get; set; } = new Address();

		public Guid? DeviceId { get; set; }

		[Obsolete("This is longer used, in favor of AuthProvider.")]
		public AuthMethod AuthMethod { get; set; }

		public string AuthProvider { get; set; }
		public String AuthID { get; set; }

		public virtual IList<SurveyResponse> SurveyResponses { get; set; }
	}

}
