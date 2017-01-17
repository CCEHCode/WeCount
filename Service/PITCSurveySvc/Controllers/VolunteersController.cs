using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using PITCSurveySvc.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
	public class VolunteersController : BaseController
    {

		// GET: api/Volunteers
		[SwaggerOperation("GetVolunteer")]
		[SwaggerResponse(HttpStatusCode.OK, "Volunteer found", typeof(VolunteerModel))]
		[ResponseType(typeof(VolunteerModel))]
		[AllowAnonymous]
		public IHttpActionResult GetVolunteer(Guid DeviceId)
        {
            Volunteer Volunteer = GetAuthenticatedVolunteer(DeviceId);

            if (Volunteer == null)
                return NotFound();

            return Ok(ModelConverter.ConvertToModel(Volunteer));
        }

		// PUT: api/Volunteers
		[SwaggerOperation("UpdateVolunteer")]
		[SwaggerResponse(HttpStatusCode.NoContent, "Volunteer updated")]
		[ResponseType(typeof(void))]
		[AllowAnonymous]
		public IHttpActionResult PutVolunteer(VolunteerModel Volunteer, Guid DeviceId)
        {
			Volunteer sv = GetAuthenticatedVolunteer(DeviceId);

			if (!ModelState.IsValid)
                return BadRequest(ModelState);

			if (sv == null)
				return BadRequest("The specified InterviewerID is not recognized. User not logged in?");

			sv.DeviceId = DeviceId;

			sv.FirstName = Volunteer.FirstName;
			sv.LastName = Volunteer.LastName;
			sv.Email = Volunteer.Email;
			sv.HomePhone = Volunteer.HomePhone;
			sv.MobilePhone = Volunteer.MobilePhone;

			sv.Address.Street = Volunteer.Address?.Street;
			sv.Address.City = Volunteer.Address?.City;
			sv.Address.State = Volunteer.Address?.State;
			sv.Address.ZipCode = Volunteer.Address?.ZipCode;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }
	}
}