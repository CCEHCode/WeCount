using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using PITCSurveySvc.Models;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
	public class VolunteersController : BaseController
    {

		// GET: api/Volunteers
		[ResponseType(typeof(VolunteerModel))]
        public IHttpActionResult GetVolunteer()
        {
            Volunteer Volunteer = GetAuthenticatedVolunteer();

            if (Volunteer == null)
            {
                return NotFound();
            }

            return Ok(ModelConverter.ConvertToModel(Volunteer));
        }

        // PUT: api/Volunteers
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVolunteer(VolunteerModel Volunteer)
        {
			Volunteer sv = GetAuthenticatedVolunteer();

			if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			if (sv == null)
			{
				return BadRequest("The specified InterviewerID is not recognized. User not logged in?");
			}

			sv.FirstName = Volunteer.FirstName;
			sv.LastName = Volunteer.LastName;
			sv.Email = Volunteer.Email;
			sv.HomePhone = Volunteer.HomePhone;
			sv.MobilePhone = Volunteer.MobilePhone;

			sv.Address.Street = Volunteer.Address.Street;
			sv.Address.City = Volunteer.Address.City;
			sv.Address.State = Volunteer.Address.State;
			sv.Address.ZipCode = Volunteer.Address.ZipCode;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

	}
}