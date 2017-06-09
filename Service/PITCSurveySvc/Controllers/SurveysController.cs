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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace PITCSurveySvc.Controllers
{
    /// <summary>
    /// Controller for Surveys (blank forms).
    /// </summary>
    public class SurveysController : BaseController
    {

        // GET: api/Surveys
        /// <summary>
        /// Get a list of available Surveys and metadata.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        [SwaggerOperation("GetAllSurveys")]
        [ResponseType(typeof(IEnumerable<SurveySummaryModel>))]
        [SwaggerResponse(HttpStatusCode.OK, "The survey summaries are being returned.", typeof(IEnumerable<SurveySummaryModel>))]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetSurveys(bool activeOnly = true)
        {
            // We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
            Volunteer sv = await GetAuthenticatedVolunteerAsync();

            var surveys = db.Surveys.Where(s => !activeOnly || s.Active);

            var models = new List<SurveySummaryModel>();

            foreach (Survey survey in surveys)
            {
                models.Add(new SurveySummaryModel()
                {
                    ID = survey.ID,
                    Description = survey.Description,
                    Version = survey.Version,
                    LastUpdated = survey.LastUpdated
                });
            }

            return Ok(models);
        }

        // GET: api/Surveys/5
        /// <summary>
        /// Get the full body of the specified Survey.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation("GetSurveyByID")]
        [ResponseType(typeof(SurveyModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, "A Survey with the specified ID was not found.")]
        [SwaggerResponse(HttpStatusCode.OK, "The specified survey is being returned.", typeof(SurveyModel))]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetSurvey(int id)
        {
            // We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
            Volunteer sv = await GetAuthenticatedVolunteerAsync();
            
            Survey survey = db.Surveys.Include("SurveyQuestions").Include("SurveyQuestions.Question").Include("SurveyQuestions.AnswerChoices").Where(s => s.ID == id).SingleOrDefault();
            if (survey == null)
                return NotFound();

            try
            {
                SurveyModel model = ModelConverter.ConvertToModel(survey);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Import
        /// <summary>
        /// Import a new / updated Survey.
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        [SwaggerOperation("PostSurvey")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NoContent, "The survey was imported.")]
        public async Task<IHttpActionResult> PostSurvey(SurveyModel survey)
        {
            // We don't use this here, but it ensures the volunteer record is created if it doesn't exist yet.
            Volunteer sv = await GetAuthenticatedVolunteerAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                ModelConverter converter = new ModelConverter(db);

                Survey importSurvey = converter.ConvertToEntity(survey);

                // Handle this like we do for child objects in converter, allows update not just insert.
                //db.Surveys.Add(Survey);

                db.SaveChanges();

                return Ok();
            }
            catch (DbEntityValidationException eve)
            {
                List<String> errors = new List<string>();

                //StringBuilder sb = new StringBuilder();

                foreach (DbEntityValidationResult vr in eve.EntityValidationErrors)
                {
                    //sb.AppendLine(vr.Entry.Entity.GetType().Name);

                    foreach (DbValidationError ve in vr.ValidationErrors)
                    {
                        string error = $"{vr.Entry.Entity.GetType().Name}.{ve.PropertyName}: {ve.ErrorMessage}";
                        //sb.AppendLine($"    {ve.PropertyName}: {ve.ErrorMessage}");
                        if (!errors.Contains(error))
                            errors.Add(error);
                    }
                }

                return InternalServerError(new InvalidOperationException(eve.Message + "\r\n" + String.Join("\r\n", errors.ToArray()), eve));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}