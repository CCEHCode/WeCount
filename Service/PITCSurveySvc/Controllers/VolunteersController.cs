using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using PITCSurveySvc.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
    public class VolunteersController : BaseController
    {

        // GET: api/Volunteers
        [SwaggerOperation("GetVolunteer")]
        [SwaggerResponse(HttpStatusCode.OK, "Volunteer found.", typeof(VolunteerModel))]
        [ResponseType(typeof(VolunteerModel))]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetVolunteer(Guid deviceId)
        {
            Volunteer volunteer = await GetAuthenticatedVolunteerAsync(deviceId);

            if (volunteer == null)
                return NotFound();

            return Ok(ModelConverter.ConvertToModel(volunteer));
        }

        // PUT: api/Volunteers
        [SwaggerOperation("UpdateVolunteer")]
        [SwaggerResponse(HttpStatusCode.NoContent, "Volunteer updated.")]
        [ResponseType(typeof(void))]
        [AllowAnonymous]
        public async Task<IHttpActionResult> PutVolunteer(VolunteerModel volunteer, Guid deviceId)
        {
            Volunteer sv = await GetAuthenticatedVolunteerAsync(deviceId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (sv == null)
                return BadRequest("The specified InterviewerID is not recognized. User not logged in?");

            sv.DeviceId = deviceId;

            sv.FirstName = volunteer.FirstName;
            sv.LastName = volunteer.LastName;
            sv.Email = volunteer.Email;
            sv.HomePhone = volunteer.HomePhone;
            sv.MobilePhone = volunteer.MobilePhone;

            sv.Address.Street = volunteer.Address?.Street;
            sv.Address.City = volunteer.Address?.City;
            sv.Address.State = volunteer.Address?.State;
            sv.Address.ZipCode = volunteer.Address?.ZipCode;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}