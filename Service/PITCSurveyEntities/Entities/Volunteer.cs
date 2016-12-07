using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PITCSurveyLib;

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
		public Address Address { get; set; }
		public AuthMethod AuthMethod { get; set; }
		public String AuthID { get; set; }

		public virtual IList<SurveyResponse> SurveyResponses { get; set; }
	}

}
