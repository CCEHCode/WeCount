using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using PITCSurveySvc.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
		/// <param name="SurveyResponse"></param>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		[ResponseType(typeof(void))]
		[SwaggerOperation("PostSurveyResponse")]
		[SwaggerResponse(HttpStatusCode.BadRequest, "The survey data wasn't acceptable (improper formatting, etc.).")]
		[SwaggerResponse(HttpStatusCode.NoContent, "SurveyResponse uploaded successfully.")]
		[AllowAnonymous]
		public async Task<IHttpActionResult> PostSurveyResponse(SurveyResponseModel surveyResponse, Guid? deviceId)
		{
			Volunteer sv = await GetAuthenticatedVolunteerAsync(deviceId);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!deviceId.HasValue)
				return BadRequest("The DeviceId was not specified.");
			else if (deviceId.Value == Guid.Empty)
				return BadRequest("The DeviceId was not valid.");

			if (sv == null)
				return BadRequest("The specified InterviewerID is not recognized. User not logged in?");

			SurveyResponse sr = db.SurveyResponses.Where(r => r.ResponseIdentifier == surveyResponse.ResponseIdentifier).SingleOrDefault();
			if (sr != null)
				// Delete current response to replace with new one
				db.SurveyResponses.Remove(sr);

			try
			{
				ModelConverter converter = new ModelConverter(db);

				SurveyResponse response = converter.ConvertToEntity(surveyResponse);

				response.Volunteer = sv;

				response.DeviceId = deviceId.Value;

				response.DateUploaded = DateTime.UtcNow;

				db.SurveyResponses.Add(response);

				db.SaveChanges();

				return StatusCode(HttpStatusCode.NoContent);
			}
			catch (DbEntityValidationException evex)
			{
				StringBuilder sb = new StringBuilder();

				foreach (var eve in evex.EntityValidationErrors)
				{
					sb.AppendLine($"{eve.Entry.Entity.GetType().Name}");

					foreach (var ve in eve.ValidationErrors)
					{
						sb.AppendLine($"{ve.PropertyName}: {ve.ErrorMessage}");
					}
				}
				Trace.TraceError("Error processing SurveyResponse: " + evex.ToString());
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, sb.ToString()));
			}
			catch (ArgumentException ae)
			{
				Trace.TraceError("Error processing SurveyResponse: " + ae.Message);
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ae.Message));
			}
			catch (FormatException fe)
			{
				Trace.TraceError("Error processing SurveyResponse: " + fe.Message);
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, fe.Message));
			}
			catch (Exception ex)
			{
				Trace.TraceError("Error processing SurveyResponse: " + ex.ToString());
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString()));
			}
		}
	}
}