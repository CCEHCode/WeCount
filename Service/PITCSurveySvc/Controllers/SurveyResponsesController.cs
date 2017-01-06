using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using PITCSurveySvc.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
	/// <summary>
	/// Controller for Survey Responses (filled-in forms).
	/// </summary>
	public class SurveyResponsesController : BaseController
    {
        
		// POST: api/SurveyResponses
		/// <summary>
		/// Submit a completed Survey Response.
		/// </summary>
		/// <param name="surveyResponse"></param>
		/// <returns></returns>
        [ResponseType(typeof(void))]
		[SwaggerOperation("Create")]
		[SwaggerResponse(HttpStatusCode.BadRequest, "The survey data wasn't acceptable (improper formatting, etc.).")]
		[SwaggerResponse(HttpStatusCode.Conflict, "A SurveyResponse with the same ResponseIdentifier is already uploaded.")]
		[SwaggerResponse(HttpStatusCode.NoContent, "SurveyResponse uploaded successfully.")]
		[AllowAnonymous]
		public IHttpActionResult PostSurveyResponse(SurveyResponseModel surveyResponse)
        {
			Volunteer sv = GetAuthenticatedVolunteer();

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

			if (sv == null)
			{
				return BadRequest("The specified InterviewerID is not recognized. User not logged in?");
			}

			try
			{
				ModelConverter Converter = new ModelConverter(db);

				SurveyResponse Response = Converter.ConvertToEntity(surveyResponse);

				Response.Volunteer = sv;

				db.SurveyResponses.Add(Response);

				db.SaveChanges();

				return StatusCode(HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}
		}
	}
}