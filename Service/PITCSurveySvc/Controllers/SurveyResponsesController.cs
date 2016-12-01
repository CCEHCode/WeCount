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
using PITCSurveyLib.Models;
using Swashbuckle.Swagger.Annotations;
using PITCSurveySvc.Models;

namespace WeCountSvc.Controllers
{
    public class SurveyResponsesController : ApiController
    {
        private PITCSurveyContext db = new PITCSurveyContext();

        // POST: api/SurveyResponses
        [ResponseType(typeof(void))]
		[SwaggerOperation("Create")]
		[SwaggerResponse(HttpStatusCode.BadRequest, "The survey data wasn't acceptable (improper formatting, etc.).")]
		[SwaggerResponse(HttpStatusCode.Conflict, "A SurveyResponse with the same ResponseIdentifier is already uploaded.")]
		[SwaggerResponse(HttpStatusCode.NoContent, "SurveyResponse uploaded successfully.")]
		public IHttpActionResult PostSurveyResponse(SurveyResponseModel surveyResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			SurveyResponse sr = db.SurveyResponses.Where(r => r.ResponseIdentifier == surveyResponse.ResponseIdentifier).SingleOrDefault();
			if (sr != null)
			{
				//return BadRequest("Survey already uploaded.");
				return StatusCode(HttpStatusCode.Conflict);
			}

			Volunteer sv = db.Volunteers.Where(v => v.AuthID == surveyResponse.InterviewerID).SingleOrDefault();
			if (sv == null)
			{
				return BadRequest("The specified InterviewerID is not recognized. User not logged in?");
				// TODO: Store and accept, figure out who it is later? Need to have way to "see" auth ID from within client app, or fill in volunteer info.

				// TODO: Create Interviewer / Volunteer db entry here and go with it? Or make this happen at app login? Probablty the latter makes morwe sense. Simpler logic & workflow.
			}

			try
			{
				ModelConverter Converter = new ModelConverter(db);

				SurveyResponse Response = Converter.ConvertToEntity(surveyResponse);

				db.SurveyResponses.Add(Response);

				db.SaveChanges();

				return StatusCode(HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}

		}

		#region "Private Methods"

		private bool SurveyResponseExists(int id)
        {
            return db.SurveyResponses.Count(e => e.ID == id) > 0;
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