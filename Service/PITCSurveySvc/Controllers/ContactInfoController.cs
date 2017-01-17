using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
    public class ContactInfoController : BaseController
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="SurveyId">If specified, will return contact info that is specific to the specified survey, if available. If survey-specific information isn't available, then the general contact info will be returned.</param>
		/// <param name="DeviceId">If specified, will return contact info that is specific to the specified user, if available. If user-specific information isn't available, then the general contact info will be returned. Note: If a user is authenticated, then that will take precendence over the DeviceId.</param>
		/// <returns></returns>
		[SwaggerOperation("GetContactInfo")]
		[ResponseType(typeof(ContactInfoModel))]
		[SwaggerResponse(HttpStatusCode.OK, "Returns the contact info, optionally specific to the specified Survey (or DeviceID, if user isn't authenticated).", typeof(ContactInfoModel))]
		[AllowAnonymous]
		public IHttpActionResult GetContactInfo(int? SurveyId = null, Guid? DeviceId = null)
		{
			ContactInfoModel Model = new ContactInfoModel()
			{
				Notes = "For help with the We Count survey project, please contact your assigned contact person." +
						" If you're not sure who that is, you can use the following general contacts." +
						"\n\nIf there is a youth in need of assistance, please contact 211." +
						"\n\nFor safety emergencies, always use 9-1-1."
			};

			Model.Contacts.Add(new ContactModel() { Name = "Brian Roccapriore", Phone = "860-555-1212" });
			Model.Contacts.Add(new ContactModel() { Name = "Omar Kouatly", Phone = "860-555-1234" });

			// We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
			Volunteer sv = GetAuthenticatedVolunteer(DeviceId);

			// TODO: We can target users, if we map them to groups or some other property in the db. Not needed for now.
			// We can even target device type, if we use relevant user-agent headers.
			// And we can target a specific survey.

			return Ok(Model);
		}
    }
}
