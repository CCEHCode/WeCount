using PITCSurveyLib.Models;
using PITCSurveySvc.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

				}

				Model.Questions.Add(qm);
			}
			
			return Model;
		}

		#endregion

		#region "Model-to-Entity Conversion"

		// These methods are not static, as they need the DbContext to pull in data for mapping.

		public SurveyResponse ConvertToEntity(SurveyResponseModel Model)
		{
			// 4326 is most common coordinate system used by GPS/Maps
			const int CoordinateSystemID = 4326;

			SurveyResponse Response = new SurveyResponse()
			{
				Survey_ID = Model.SurveyID,
				Volunteer_ID = _db.Volunteers.Where(v => v.AuthID == Model.InterviewerID).Single().ID,
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