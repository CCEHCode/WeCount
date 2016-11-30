using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PITCSurveySvc.Entities;

namespace WeCountSvc.Controllers
{
    public class SurveyResponsesController : ApiController
    {
        private WeCountContext db = new WeCountContext();

        // GET: api/SurveyResponses
        public IQueryable<SurveyResponse> GetSurveyResponses()
        {
            return db.SurveyResponses;
        }

        // GET: api/SurveyResponses/5
        [ResponseType(typeof(SurveyResponse))]
        public IHttpActionResult GetSurveyResponse(int id)
        {
            SurveyResponse surveyResponse = db.SurveyResponses.Find(id);
            if (surveyResponse == null)
            {
                return NotFound();
            }

            return Ok(surveyResponse);
        }

        // PUT: api/SurveyResponses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSurveyResponse(int id, SurveyResponse surveyResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != surveyResponse.ID)
            {
                return BadRequest();
            }

            db.Entry(surveyResponse).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyResponseExists(id))
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

        // POST: api/SurveyResponses
        [ResponseType(typeof(SurveyResponse))]
        public IHttpActionResult PostSurveyResponse(SurveyResponse surveyResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SurveyResponses.Add(surveyResponse);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = surveyResponse.ID }, surveyResponse);
        }

        // DELETE: api/SurveyResponses/5
        [ResponseType(typeof(SurveyResponse))]
        public IHttpActionResult DeleteSurveyResponse(int id)
        {
            SurveyResponse surveyResponse = db.SurveyResponses.Find(id);
            if (surveyResponse == null)
            {
                return NotFound();
            }

            db.SurveyResponses.Remove(surveyResponse);
            db.SaveChanges();

            return Ok(surveyResponse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SurveyResponseExists(int id)
        {
            return db.SurveyResponses.Count(e => e.ID == id) > 0;
        }
    }
}