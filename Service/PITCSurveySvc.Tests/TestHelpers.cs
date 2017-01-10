using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomGen;
using EfficientlyLazy.IdentityGenerator;

namespace PITCSurveySvc.Tests
{
	static class TestHelpers
	{
		private static Random Rnd = new Random();

		private static Func<int> Ints = Gen.Random.Numbers.Integers(1, 100);
		private static Func<DateTime> Dates = Gen.Random.Time.Dates(DateTime.Now.AddYears(-90), DateTime.Now.AddYears(-12));
		private static Func<string> Words = Gen.Random.Text.Words();
		private static Func<string> Names = Gen.Random.Names.Full();

		public static SurveyResponseModel GenerateRandomResponseForSurvey(Survey Survey)
		{
			var Addr = Generator.GenerateAddress();

			SurveyResponseModel Responses = new SurveyResponseModel()
			{
				StartTime = DateTime.Now.AddMinutes(-7),
				EndTime = DateTime.Now,
				GPSLocation = new PITCSurveyLib.GPSLocation() { Lat = 47.6419587, Lon = -122.1327818, Accuracy = 0 },
				NearestAddress = new PITCSurveyLib.Address()
				{
					Street = Addr.AddressLine,
					City = Addr.City,
					State = Addr.StateAbbreviation,
					ZipCode = Addr.ZipCode
				},
				LocationNotes = "",
				SurveyID = 1,
				Survey_Version = Survey.Version,
				ResponseIdentifier = Guid.NewGuid(),
			};

			foreach (var Question in Survey.SurveyQuestions)
			{
				var Response = new SurveyQuestionResponseModel() { QuestionID = Question.ID };

				int AnswersToSelect = Question.Question.AllowMultipleAnswers ? Rnd.Next(1, Question.AnswerChoices.Count() / 4) : 1;

				for (int i = 0; i < AnswersToSelect; i++)
				{
					SurveyAnswerChoice Choice = Question.AnswerChoices[i];

					SurveyQuestionAnswerChoiceResponseModel Answer = new SurveyQuestionAnswerChoiceResponseModel()
					{
						QuestionID = Question.Question.ID,
						AnswerChoiceID = Choice.AnswerChoice.ID
					};

					switch (Choice.AnswerChoice.AdditionalAnswerDataFormat)
					{
						case PITCSurveyLib.AnswerFormat.Int:
							Answer.AdditionalAnswerData = Ints().ToString();
							break;

						case PITCSurveyLib.AnswerFormat.Date:
							Answer.AdditionalAnswerData = Dates().ToShortDateString();
							break;

						case PITCSurveyLib.AnswerFormat.String:
							if (Question.Question.WellKnownQuestion == PITCSurveyLib.WellKnownQuestion.NameOrInitials)
							{
								Answer.AdditionalAnswerData = Names();
							}
							else
								Answer.AdditionalAnswerData = Words();
							break;

						default:
							break;
					}

					Response.AnswerChoiceResponses.Add(Answer);
				}

				Responses.QuestionResponses.Add(Response);
			}

			return Responses;
		}
	}
}
