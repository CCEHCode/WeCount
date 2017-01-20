using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
    public class ContactInfoController : BaseController
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surveyId">If specified, will return contact info that is specific to the specified survey, if available. If survey-specific information isn't available, then the general contact info will be returned.</param>
		/// <param name="deviceId">If specified, will return contact info that is specific to the specified user, if available. If user-specific information isn't available, then the general contact info will be returned. Note: If a user is authenticated, then that will take precendence over the DeviceId.</param>
		/// <returns></returns>
		[SwaggerOperation("GetContactInfo")]
		[ResponseType(typeof(ContactInfoModel))]
		[SwaggerResponse(HttpStatusCode.OK, "Returns the contact info, optionally specific to the specified Survey (or DeviceID, if user isn't authenticated).", typeof(ContactInfoModel))]
		[AllowAnonymous]
		public async Task<IHttpActionResult> GetContactInfo(int? surveyId = null, Guid? deviceId = null)
		{
			ContactInfo info = null;

			var model = new ContactInfoModel();

			// We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
			Volunteer sv = await GetAuthenticatedVolunteerAsync(deviceId);

			// We can target a specific survey.
			if (surveyId.HasValue)
			{
				info = db.Surveys.Where(s => s.ID == surveyId.Value).SingleOrDefault()?.ContactInfo;
			}
			else
			{
				// For now, we're hard-coding this.
				info = db.ContactInfos.Where(i => i.ID == 1).SingleOrDefault();
			}

			if (info != null)
			{
				model.Notes = info.Notes;

				foreach (Contact contact in info.Contacts)
				{
					model.Contacts.Add(new ContactModel() { Name = contact.Name, Phone = contact.PhoneNumber });
				}
			}

			// TODO: We can target users, if we map them to groups or some other property in the db. Not needed for now.
			// We can even target device type, if we use relevant user-agent headers.

			return Ok(model);
		}
    }
}
