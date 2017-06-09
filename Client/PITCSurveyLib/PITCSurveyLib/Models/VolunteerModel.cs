using System;

namespace PITCSurveyLib.Models
{
    public class VolunteerModel
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String MobilePhone { get; set; }
        public String HomePhone { get; set; }
        public Address Address { get; set; } = new Address();
    }
}
