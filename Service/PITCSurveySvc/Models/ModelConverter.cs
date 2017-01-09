using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using System;
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
				IntroText = Survey.IntroText,
				Version = Survey.Version,
				LastUpdated = Survey.LastUpdated
			};

			foreach (SurveyQuestion sq in Survey.SurveyQuestions)
			{
				SurveyQuestionModel qm = new SurveyQuestionModel()
				{
					QuestionID = sq.Question_ID,
					QuestionNum = sq.QuestionNum,
					QuestionText = sq.Question.QuestionText,
					QuestionHelpText = sq.Question.ClarificationText,
					AllowMultipleAnswers = sq.Question.AllowMultipleAnswers,
					WellKnownQuestion = sq.Question.WellKnownQuestion
				};

				foreach (SurveyAnswerChoice ac in sq.AnswerChoices)
				{
					SurveyQuestionAnswerChoiceModel acm = new SurveyQuestionAnswerChoiceModel()
					{
						AnswerChoiceID = ac.AnswerChoice_ID,
						AnswerChoiceNum = ac.AnswerChoiceNum,
						AnswerChoiceText = ac.AnswerChoice.AnswerText,
						AdditionalAnswerDataFormat = ac.AnswerChoice.AdditionalAnswerDataFormat,
						NextQuestionID = (ac.EndSurvey) ? SurveyQuestionAnswerChoiceModel.END_SURVEY : ac.NextSurveyQuestion_ID,
						EndSurvey = ac.EndSurvey
					};

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
			Survey Survey = _db.Surveys.Where(s => s.Description == Model.Description).SingleOrDefault();

			if (Survey == null)
			{
				Survey = new Survey()
				{
					SurveyQuestions = new List<SurveyQuestion>()
				};

				_db.Surveys.Add(Survey);
			}

			// For now, ignore ID - assume is new. We can delete existing if re-importing.
			Survey.Name = Model.Name;
			Survey.Description = Model.Description;
			Survey.IntroText = Model.IntroText;

			Survey.LastUpdated = DateTime.UtcNow;
			Survey.Version += 1;

			// Map the provided IDs to the existing or db-generated questions, to preserve navigation mapping
			Dictionary<int, Question> QuestionsByModelID = new Dictionary<int, Question>();
			Dictionary<int, AnswerChoice> AnswerChoicesByModelID = new Dictionary<int, AnswerChoice>();

			// Process all questions and answer choices first, so we have them in the db and indexed by model ID.

			foreach (SurveyQuestionModel qm in Model.Questions)
			{
				// See if question already exists. Remember, questions are reusable, and can be shared across surveys.

				Question q = _db.Questions.WhereEx(eq => eq.QuestionText == qm.QuestionText).SingleOrDefault();
				
				if (q == null)
				{
					q = new Question();

					_db.Questions.Add(q);

					Trace.WriteLine($"+ Q {q.QuestionText}");
				}
				else
				{
					Trace.WriteLine($"- Q {q.QuestionText}");
				}

				q.QuestionText = qm.QuestionText;
				q.ClarificationText = qm.QuestionHelpText;            // Move to SurveyQuestion?
				q.AllowMultipleAnswers = qm.AllowMultipleAnswers;      // Move to SurveyQuestion?
				q.WellKnownQuestion = qm.WellKnownQuestion;

				QuestionsByModelID.Add(qm.QuestionID, q);

				foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
				{
					// See if answer choice already exists. Remember, answer choices are reusable, and can be shared across questions and surveys.

					AnswerChoice a = _db.AnswerChoices.WhereEx(ac => ac.AnswerText == acm.AnswerChoiceText && ac.AdditionalAnswerDataFormat == acm.AdditionalAnswerDataFormat).SingleOrDefault();

					if (a == null)
					{
						a = new AnswerChoice();

						_db.AnswerChoices.Add(a);

						Trace.WriteLine($"    + AC {a.AnswerText}");
					}
					else
					{
						Trace.WriteLine($"    - AC {a.AnswerText}");
					}

					a.AnswerText = acm.AnswerChoiceText;
					a.AdditionalAnswerDataFormat = acm.AdditionalAnswerDataFormat;

					if (!AnswerChoicesByModelID.ContainsKey(acm.AnswerChoiceID))
					{
						AnswerChoicesByModelID.Add(acm.AnswerChoiceID, a);
					}
				}

			}

			// Now we can process the model into the survey-specific entities.
			// TODO: This block can now be moved into previous
			foreach (SurveyQuestionModel qm in Model.Questions)
			{

				Question q = QuestionsByModelID[qm.QuestionID];

				SurveyQuestion sq = Survey.SurveyQuestions.Where(sq2 => sq2.Question == q).SingleOrDefault();

				if (sq == null)
				{
					sq = new SurveyQuestion
					{
						Question = q,
						AnswerChoices = new List<SurveyAnswerChoice>()
					};

					Survey.SurveyQuestions.Add(sq);

					_db.SurveyQuestions.Add(sq);
				}

				sq.QuestionNum = qm.QuestionNum;
			}

			// Finally, with the SurveyQuestions all added and mapped, we can process AnswerChoices with forward-referenced NextQuestionID nav property

			foreach (SurveyQuestionModel qm in Model.Questions)
			{

				Question q = QuestionsByModelID[qm.QuestionID];

				SurveyQuestion sq = Survey.SurveyQuestions.Where(sq2 => sq2.Question == q).SingleOrDefault();

				foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
				{
					// See if survey answer choice already exists.

					SurveyAnswerChoice sac = sq.AnswerChoices.Where(c => c.AnswerChoice == AnswerChoicesByModelID[acm.AnswerChoiceID]).SingleOrDefault();

					if (sac == null)
					{
						sac = new SurveyAnswerChoice
						{
							AnswerChoice = AnswerChoicesByModelID[acm.AnswerChoiceID],
						};

						sq.AnswerChoices.Add(sac);

						_db.SurveyAnswerChoices.Add(sac);
					}

					sac.AnswerChoiceNum = acm.AnswerChoiceNum;

					if (acm.NextQuestionID.HasValue && acm.NextQuestionID.Value != -1)
					{
						try
						{
							sac.NextSurveyQuestion = Survey.SurveyQuestions.Where(ssq => ssq.Question == QuestionsByModelID[acm.NextQuestionID.Value]).Single();
						}
						catch (Exception ex)
						{
							throw new InvalidOperationException($"Couldn't find NSQ {acm.NextQuestionID} (Q='{q.QuestionText}', AC='{acm.AnswerChoiceText}')", ex);
						}
					}
					else if (acm.NextQuestionID.HasValue && acm.NextQuestionID.Value == -1)
					{
						sac.EndSurvey = true;
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
				Survey_Version = Model.Survey_Version,
				//Volunteer_ID = _db.Volunteers.Where(v => v.AuthID == Model.InterviewerID).Single().ID,
				GPSLocation = (Model.GPSLocation != null) ? System.Data.Entity.Spatial.DbGeography.PointFromText($"Point({Model.GPSLocation.Lon} {Model.GPSLocation.Lat})", CoordinateSystemID) : null,
				LocationNotes = Model.LocationNotes,
				NearestAddress = Model.NearestAddress ?? new PITCSurveyLib.Address(),
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