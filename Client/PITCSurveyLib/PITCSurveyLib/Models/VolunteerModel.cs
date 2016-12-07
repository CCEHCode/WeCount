using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITCSurveyLib.Models
{
	public class VolunteerModel
	{
		public String FirstName { get; set; }
		public String LastName { get; set; }
		public String Email { get; set; }
		public String MobilePhone { get; set; }
		public String HomePhone { get; set; }
		public Address Address { get; set; }
	}
}
