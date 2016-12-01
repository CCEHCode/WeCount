using PITCSurveyLib.Models;
using PITCSurveySvc.Entities;
using PITCSurveySvc.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

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
			Survey Survey = db.Surveys.Find(id);
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
				Survey Svy = new Survey
				{
					// For now, ignore ID - assume is new. We can delete existing if re-importing.
					Name = Model.Name,
					Description = Model.Description,
					IntroText = Model.IntroText,
					SurveyQuestions = new List<SurveyQuestion>()
				};

				// Map the provided IDs to the existing or db-generated questions, to preserve navigation mapping
				Dictionary<int, Question> QuestionsByModelID = new Dictionary<int, Question>();
				Dictionary<int, AnswerChoice> AnswerChoicesByModelID = new Dictionary<int, AnswerChoice>();

				foreach (SurveyQuestionModel qm in Model.Questions)
				{
					// See if question already exists. Remember, questions are reusable, and can be shared across surveys.

					Question q = db.Questions.Where(eq => eq.QuestionText == qm.QuestionText).SingleOrDefault();

					if (q == null)
					{
						q = new Question
						{
							QuestionText = qm.QuestionText,
							ClarificationText = qm.QuestionHelpText,
							AllowMultipleAnswers = qm.AllowMultipleAnswers,
							AnswerChoices = new List<AnswerChoice>()
						};

						db.Questions.Add(q);
						QuestionsByModelID.Add(qm.QuestionID, q);
					}

					foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
					{
						// See if answer choice already exists. Remember, answer choices are reusable, and can be shared across questions and surveys.

						AnswerChoice a = db.AnswerChoices.Where(ac => ac.AnswerText == acm.AnswerChoiceText).SingleOrDefault();

						if (a == null)
						{
							a = new AnswerChoice
							{
								AnswerText = acm.AnswerChoiceText,
								AdditionalAnswerDataFormat = acm.AdditionalAnswerDataFormat
							};

							db.AnswerChoices.Add(a);
							AnswerChoicesByModelID.Add(acm.AnswerChoiceID, a);
						}

						q.AnswerChoices.Add(a);
					}

					Svy.SurveyQuestions.Add(new SurveyQuestion
					{
						Question = q
					});
				}

				// Save changes to get DB IDs for new items, so we can process navigation

				db.SaveChanges();

				// Now process navigation, mapping the model IDs to the actual DB IDs

				foreach (SurveyQuestionModel qm in Model.Questions)
				{
					Question q = db.Questions.Where(eq => eq.QuestionText == qm.QuestionText).Single();

					foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
					{
						if (acm.NextQuestionID.HasValue)
						{
							AnswerChoice a = db.AnswerChoices.Where(ac => ac.AnswerText == acm.AnswerChoiceText).Single();
							SurveyQuestion sq = Svy.SurveyQuestions.Where(nsq => nsq.Question_ID == QuestionsByModelID[qm.QuestionID].ID).Single();

							SurveyNavigation nav = new SurveyNavigation { AnswerChoice_ID = a.ID, NextSurveyQuestion_ID = QuestionsByModelID[acm.NextQuestionID.Value].ID };

							db.SurveyNavigation.Add(nav);
							sq.Navigation.Add(nav);
						}

					}
				}

				return Ok();
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