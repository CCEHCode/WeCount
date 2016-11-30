using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PITCSurveyLib;

namespace PITCSurveySvc.Entities
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
		public Address Address { get; set; }
		public AuthMethod AuthMethod { get; set; }
		public String AuthID { get; set; }

		public virtual IList<SurveyResponse> SurveyResponses { get; set; }
	}

	public class Address
	{
		public String Street { get; set; }
		public String City { get; set; }
		public String State { get; set; }
		public String ZipCode { get; set; }
	}
}
