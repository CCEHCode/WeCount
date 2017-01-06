using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PITCSurveySvc.Models;
using PITCSurveyEntities.Entities;
using System.Web.Script.Serialization;
using PITCSurveyLib.Models;
using System.IO;
using System.Linq;

namespace PITCSurveySvc.Tests
{
	[TestClass]
	public class ModelConverterTests
	{
		[TestMethod]
		public void TestImportSurveyJSON()
		{
			using (PITCSurveyContext db = new PITCSurveyContext())
			{
				ModelConverter mc = new ModelConverter(db);

				var js = new JavaScriptSerializer();

				SurveyModel Model = js.Deserialize<SurveyModel>(File.ReadAllText(".\\Data\\PITYouthCountSurvey.json"));

				Survey Survey = mc.ConvertToEntity(Model);

				Console.WriteLine();
			}
		}

		[TestMethod]
		public void TestBuildResponseJSON()
		{
			using (var db = new PITCSurveyEntities.Entities.PITCSurveyContext())
			{
				var Survey = db.Surveys.Where(s => s.ID == 1).Single();

				SurveyResponseModel Responses = new SurveyResponseModel()
				{
					StartTime = DateTime.Now.AddMinutes(-7),
					EndTime = DateTime.Now,
					GPSLocation = new PITCSurveyLib.GPSLocation() { Lat = 47.6419587, Lon = -122.1327818, Accuracy = 0 },
					NearestAddress = new PITCSurveyLib.Address() { Street = "", City = "", State = "", ZipCode = "" },
					LocationNotes = "",
					SurveyID = 1,
					ResponseIdentifier = Guid.NewGuid(),
				};

				Random Rnd = new Random();

				foreach (var Question in Survey.SurveyQuestions)
				{
					var Response = new SurveyQuestionResponseModel() { QuestionID = Question.ID };

					int AnswersToSelect = Question.Question.AllowMultipleAnswers ? Rnd.Next(1, Question.AnswerChoices.Count() / 4) : 1;

					for (int i = 0; i < AnswersToSelect; i++)
					{
						SurveyAnswerChoice Choice = Question.AnswerChoices[i];

						SurveyQuestionAnswerChoiceResponseModel Answer = new SurveyQuestionAnswerChoiceResponseModel()
						{
							QuestionID = Question.ID,
							AnswerChoiceID = Choice.AnswerChoice.ID
						};

						switch (Choice.AnswerChoice.AdditionalAnswerDataFormat)
						{
							case PITCSurveyLib.AnswerFormat.Int:
								Answer.AdditionalAnswerData = Rnd.Next(1, 100).ToString();
								break;

							case PITCSurveyLib.AnswerFormat.Date:
								Answer.AdditionalAnswerData = DateTime.Now.AddDays(0 - Rnd.Next(1, 365)).ToShortDateString();
								break;

							case PITCSurveyLib.AnswerFormat.String:
								Answer.AdditionalAnswerData = "Random words here";
								break;

							default:
								break;
						}
						
						Response.AnswerChoiceResponses.Add(Answer);
					}

					Responses.QuestionResponses.Add(Response);
				}

				var js = new JavaScriptSerializer();

				System.Diagnostics.Trace.WriteLine(js.Serialize(Responses));
			}
		}
	}
}
