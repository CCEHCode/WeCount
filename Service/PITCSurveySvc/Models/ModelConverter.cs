using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PITCSurveySvc.Models
{
	/// <summary>
	/// Converts between JSON models and EF database entities.
	/// Entity-to-Model conversion is static, since EF lazy-load will provide the object graph needed.
	/// Model-to-Entity conversion is instance and needs DbContext, so it can pull mapping data from the db.
	/// </summary>
	public class ModelConverter
	{
		private PITCSurveyContext _db;
		
		public ModelConverter(PITCSurveyContext db)
		{
			_db = db;
		}

		#region "Entity-to-Model Conversion (Static)"

		public static SurveyModel ConvertToModel(Survey Survey)
		{
			SurveyModel Model = new SurveyModel()
			{
				SurveyID = Survey.ID,
				Name = Survey.Name,
				Description = Survey.Description,
				IntroText = Survey.IntroText
			};

			foreach (SurveyQuestion sq in Survey.SurveyQuestions)
			{
				SurveyQuestionModel qm = new SurveyQuestionModel()
				{
					QuestionID = sq.Question_ID,
					QuestionNum = sq.QuestionNum,
					QuestionText = sq.Question.QuestionText,
					QuestionHelpText = sq.Question.ClarificationText,
					AllowMultipleAnswers = sq.Question.AllowMultipleAnswers
				};

				foreach (AnswerChoice ac in sq.Question.AnswerChoices)
				{
					SurveyQuestionAnswerChoiceModel acm = new SurveyQuestionAnswerChoiceModel()
					{
						AnswerChoiceID = ac.ID,
						AnswerChoiceNum = ac.ID.ToString(),
						AnswerChoiceText = ac.AnswerText,
						AdditionalAnswerDataFormat = ac.AdditionalAnswerDataFormat
					};

					SurveyNavigation sn = sq.Navigation.Where(n => n.AnswerChoice_ID == ac.ID).SingleOrDefault();

					if (sn != null)
						acm.NextQuestionID = sn.NextSurveyQuestion_ID;

					qm.AnswerChoices.Add(acm);
				}

				Model.Questions.Add(qm);
			}
			
			return Model;
		}

		public static VolunteerModel ConvertToModel(Volunteer Volunteer)
		{
			VolunteerModel Model = new VolunteerModel()
			{
				FirstName = Volunteer.FirstName,
				LastName = Volunteer.LastName,
				Email = Volunteer.Email,
				HomePhone = Volunteer.HomePhone,
				MobilePhone = Volunteer.MobilePhone
			};

			Model.Address.Street = Volunteer.Address.Street;
			Model.Address.City = Volunteer.Address.City;
			Model.Address.State = Volunteer.Address.State;
			Model.Address.ZipCode = Volunteer.Address.ZipCode;

			return Model;
		}

		#endregion

		#region "Model-to-Entity Conversion"

		// These methods are not static, as they need the DbContext to pull in data for mapping.
		public Survey ConvertToEntity(SurveyModel Model)
		{
			Survey Survey = new Survey
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

				Question q = _db.Questions.WhereEx(eq => eq.QuestionText == qm.QuestionText).SingleOrDefault();

				if (q == null)
				{
					q = new Question
					{
						QuestionText = qm.QuestionText,
						ClarificationText = qm.QuestionHelpText,
						AllowMultipleAnswers = qm.AllowMultipleAnswers,
						AnswerChoices = new List<AnswerChoice>()
					};

					_db.Questions.Add(q);
					QuestionsByModelID.Add(qm.QuestionID, q);

					Trace.WriteLine($"+ Q {q.QuestionText}");
				}
				else
				{
					Trace.WriteLine($"- Q {q.QuestionText}");
				}

				foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
				{
					// See if answer choice already exists. Remember, answer choices are reusable, and can be shared across questions and surveys.

					AnswerChoice a = null;

					if (AnswerChoicesByModelID.ContainsKey(acm.AnswerChoiceID))
					{
						a = AnswerChoicesByModelID[acm.AnswerChoiceID];

						Trace.WriteLine($"    - AC {a.AnswerText}");
					}
					else
					{
						a = _db.AnswerChoices.WhereEx(ac => ac.AnswerText == acm.AnswerChoiceText).SingleOrDefault();

						if (a == null)
						{
							a = new AnswerChoice
							{
								AnswerText = acm.AnswerChoiceText,
								AdditionalAnswerDataFormat = acm.AdditionalAnswerDataFormat
							};

							_db.AnswerChoices.Add(a);
							AnswerChoicesByModelID.Add(acm.AnswerChoiceID, a);

							Trace.WriteLine($"    + AC {a.AnswerText}");
						}
					}

					q.AnswerChoices.Add(a);
				}

				Survey.SurveyQuestions.Add(new SurveyQuestion
				{
					Question = q
				});
			}

			// Save changes to get DB IDs for new items, so we can process navigation

			//_db.SaveChanges();

			// Now process navigation, mapping the model IDs to the actual DB IDs

			foreach (SurveyQuestionModel qm in Model.Questions)
			{
				Question q = _db.Questions.WhereEx(eq => eq.QuestionText == qm.QuestionText).Single();

				foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
				{
					if (acm.NextQuestionID.HasValue)
					{
						AnswerChoice a = _db.AnswerChoices.WhereEx(ac => ac.AnswerText == acm.AnswerChoiceText).Single();
						SurveyQuestion sq = Survey.SurveyQuestions.Where(qq => qq.Question == QuestionsByModelID[qm.QuestionID]).Single();

						SurveyNavigation nav = new SurveyNavigation { SurveyQuestion = sq, AnswerChoice = a, NextSurveyQuestion = sq};

						_db.SurveyNavigation.Add(nav);
						sq.Navigation.Add(nav);
					}

				}
			}

			return Survey;
		}

		public SurveyResponse ConvertToEntity(SurveyResponseModel Model)
		{
			// 4326 is most common coordinate system used by GPS/Maps
			const int CoordinateSystemID = 4326;

			SurveyResponse Response = new SurveyResponse()
			{
				Survey_ID = Model.SurveyID,
				//Volunteer_ID = _db.Volunteers.Where(v => v.AuthID == Model.InterviewerID).Single().ID,
				GPSLocation = System.Data.Entity.Spatial.DbGeography.PointFromText($"POINT({Model.GPSLocation.Lon} {Model.GPSLocation.Lat}", CoordinateSystemID),
				LocationNotes = Model.LocationNotes,
				NearestAddress = Model.NearestAddress,
				ResponseIdentifier = Model.ResponseIdentifier,
				InterviewStarted = Model.StartTime,
				InterviewCompleted = Model.EndTime
			};

			foreach (SurveyQuestionResponseModel qrm in Model.QuestionResponses)
			{
				foreach (SurveyQuestionAnswerChoiceResponseModel qacrm in qrm.AnswerChoiceResponses)
				{
					SurveyResponseAnswer Answer = new SurveyResponseAnswer()
					{
						Question_ID = qrm.QuestionID,
						AnswerChoice_ID = qacrm.AnswerChoiceID,
						AdditionalAnswerData = qacrm.AdditionalAnswerData
					};

					Response.Answers.Add(Answer);
				}
			}

			return Response;
		}

		#endregion

	}
}