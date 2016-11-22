using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITCSurveyApp.Lib.Model
{
    class Volunteer
    {
        public enum AuthenticationMethod
        {
            Microsoft = 1, Google = 2
        }

        // Required fields
        public string AuthId { get; set; }  // The Auth ID for the chosen Auth method
        public AuthenticationMethod AuthMethod { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }

        // Optional Fields
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
