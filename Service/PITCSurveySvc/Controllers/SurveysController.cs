using PITCSurveyLib.Models;
using PITCSurveyEntities.Entities;
using PITCSurveySvc.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity.Validation;
using System.Text;

namespace WeCountSvc.Controllers
{
	public class SurveysController : ApiController
    {
        private PITCSurveyContext db = new PITCSurveyContext();

		// GET: api/Surveys
		[SwaggerOperation("GetAll")]
		[ResponseType(typeof(IEnumerable<SurveyModel>))]
		[SwaggerResponse(HttpStatusCode.OK, "All mai Survey are belong 2 u <3", typeof(IEnumerable<SurveyModel>))]
		public IEnumerable<SurveyModel> GetSurveys(bool ActiveOnly)
        {
			var Surveys = db.Surveys.Where(s => !ActiveOnly || s.Active);

			var Models = new List<SurveyModel>();

			foreach (Survey Survey in Surveys)
			{
				Models.Add(ModelConverter.ConvertToModel(Survey));
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
			try
			{

				ModelConverter Converter = new ModelConverter(db);

				Survey Survey = Converter.ConvertToEntity(Model);

				db.Surveys.Add(Survey);

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

		#region "Private Methods"

		private bool SurveyExists(int id)
		{
			return db.Surveys.Count(e => e.ID == id) > 0;
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