using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using PITCSurveySvc.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
	public class SurveysController : BaseController
    {

		// GET: api/Surveys
		[SwaggerOperation("GetAll")]
		[ResponseType(typeof(IEnumerable<SurveyModel>))]
		[SwaggerResponse(HttpStatusCode.OK, "All mai Survey are belong 2 u <3", typeof(IEnumerable<SurveyModel>))]
		public IEnumerable<SurveySummaryModel> GetSurveys(bool ActiveOnly)
        {
			// We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
			Volunteer sv = GetAuthenticatedVolunteer();

			var Surveys = db.Surveys.Where(s => !ActiveOnly || s.Active);

			var Models = new List<SurveySummaryModel>();

			foreach (Survey Survey in Surveys)
			{
				Models.Add(new SurveySummaryModel()
				{
					ID = Survey.ID,
					Description = Survey.Description,
					Version = Survey.Version,
					LastUpdated = Survey.LastUpdated
				});
			}

			return Models;
        }

		// GET: api/Surveys/5
		[SwaggerOperation("GetByID")]
		[ResponseType(typeof(SurveyModel))]
		[SwaggerResponse(HttpStatusCode.NotFound, "A Survey with the specified ID was not found.")]
		[SwaggerResponse(HttpStatusCode.OK, "U can haz Survey", typeof(SurveyModel))]
		public IHttpActionResult GetSurvey(int id)
		{
			// We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
			Volunteer sv = GetAuthenticatedVolunteer();

			Survey Survey = db.Surveys.Where(s => s.ID == id).SingleOrDefault();
			if (Survey == null)
			{
				return NotFound();
			}
			else
			{
				try
				{
					SurveyModel Model = ModelConverter.ConvertToModel(Survey);

					return Ok(Model);
				}
				catch (Exception ex)
				{
					return InternalServerError(ex);
				}
			}
        }

		// Import
		[SwaggerOperation("Create")]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		public IHttpActionResult PostSurvey(SurveyModel Model)
		{
			// We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
			Volunteer sv = GetAuthenticatedVolunteer();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			else
			{
				try
				{

					ModelConverter Converter = new ModelConverter(db);

					Survey Survey = Converter.ConvertToEntity(Model);

					// Handle this like we do for child objects in converter, allows update not just insert.
					//db.Surveys.Add(Survey);

					db.SaveChanges();

					return Ok();
				}
				catch (DbEntityValidationException eve)
				{
					List<String> Errors = new List<string>();

					//StringBuilder sb = new StringBuilder();

					foreach (DbEntityValidationResult vr in eve.EntityValidationErrors)
					{
						//sb.AppendLine(vr.Entry.Entity.GetType().Name);

						foreach (DbValidationError ve in vr.ValidationErrors)
						{
							string Error = $"{vr.Entry.Entity.GetType().Name}.{ve.PropertyName}: {ve.ErrorMessage}";
							//sb.AppendLine($"    {ve.PropertyName}: {ve.ErrorMessage}");
							if (!Errors.Contains(Error))
								Errors.Add(Error);
						}
					}

					return InternalServerError(new InvalidOperationException(eve.Message + "\r\n" + String.Join("\r\n", Errors.ToArray()), eve));
				}
				catch (Exception ex)
				{
					return InternalServerError(ex);
				}
			}
		}

    }
}