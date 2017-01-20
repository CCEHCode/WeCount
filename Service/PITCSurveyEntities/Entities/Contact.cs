using System.ComponentModel.DataAnnotations;

namespace PITCSurveyEntities.Entities
{
	public class Contact
	{
		[Key]
		public int ID { get; set; }

		public string Name { get; set; }
		public string PhoneNumber { get; set; }

		public ContactInfo ContactInfo { get; set; }
	}
}
