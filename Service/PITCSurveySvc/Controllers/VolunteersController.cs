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
            Volunteer volunteer = GetAuthenticatedVolunteer();

            if (volunteer == null)
            {
                return NotFound();
            }

            return Ok(ModelConverter.ConvertToModel(volunteer));
        }

        // PUT: api/Volunteers
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVolunteer(VolunteerModel Model)
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

			sv.FirstName = Model.FirstName;
			sv.LastName = Model.LastName;
			sv.Email = Model.Email;
			sv.HomePhone = Model.HomePhone;
			sv.MobilePhone = Model.MobilePhone;

			sv.Address.Street = Model.Address.Street;
			sv.Address.City = Model.Address.City;
			sv.Address.State = Model.Address.State;
			sv.Address.ZipCode = Model.Address.ZipCode;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

	}
}