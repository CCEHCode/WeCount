using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PITCSurveyEntities.Entities;
using PITCSurveyLib;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace PITCSurveySvc.Controllers
{
    public class VolunteersController : ApiController
    {
        private PITCSurveyContext db = new PITCSurveyContext();

		// GET: api/Volunteers/5
		[ResponseType(typeof(Volunteer))]
        public IHttpActionResult GetVolunteer(int id)
        {
            Volunteer volunteer = db.Volunteers.Find(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return Ok(volunteer);
        }

        // PUT: api/Volunteers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVolunteer(int id, Volunteer volunteer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != volunteer.ID)
            {
                return BadRequest();
            }

            db.Entry(volunteer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolunteerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Volunteers
        [ResponseType(typeof(Volunteer))]
        public IHttpActionResult PostVolunteer(Volunteer volunteer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Volunteers.Add(volunteer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = volunteer.ID }, volunteer);
        }

		#region "Private Methods"

		private bool VolunteerExists(int id)
		{
			return db.Volunteers.Count(e => e.ID == id) > 0;
		}

		#endregion

		#region "IDisposable"

		protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

		#endregion

	}
}