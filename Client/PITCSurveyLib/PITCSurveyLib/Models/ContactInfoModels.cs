using System.Collections.Generic;

namespace PITCSurveyLib.Models
{
	/// <summary>
	/// Client-side model to retrieve contact info.
	/// </summary>
	public class ContactInfoModel
	{
		/// <summary>
		/// General notes to be displayed along with contact info.
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// A list of contacts.
		/// </summary>
		public IList<ContactModel> Contacts { get; set; } = new List<ContactModel>();
	}

	/// <summary>
	/// Client-side model for a contact's info.
	/// </summary>
	public class ContactModel
	{
		/// <summary>
		/// Contact name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Contact's phone number.
		/// </summary>
		public string Phone { get; set; }
	}
}
