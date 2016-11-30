using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PITCSurveySvc.Entities;
using Effort;
using System.Data.Common;
using System.Linq;
using PITCSurveyLib.Models;

namespace PITCSurveySvc.Tests
{
	[TestClass]
	public class UnitTest1
	{
		private WeCountContext _db;

		[TestInitialize]
		public void Init()
		{
			EffortProviderFactory.ResetDB();
		}

		public UnitTest1()
		{
			//DbConnection conn = DbConnectionFactory.CreateTransient();
			//_db = new WeCountContext(conn);
			_db = new WeCountContext();
		}

		[TestMethod]
		public void TestMethod1()
		{
			SurveyModel sm = new SurveyModel { SurveyID = 1, Description = "2017 PIT Youth Survey" };

			var q1 = new SurveyQuestionModel { QuestionID = 1, QuestionNum = "1", QuestionText = "What's your favorite color?", QuestionHelpText = "Please limit to Crayola-named colors." };

			q1.AnswerChoices.Add(new SurveyQuestionAnswerChoiceModel { AnswerChoiceID = 1, AnswerChoiceNum = "a", AnswerChoiceText = "Red" });
			q1.AnswerChoices.Add(new SurveyQuestionAnswerChoiceModel { AnswerChoiceID = 2, AnswerChoiceNum = "b", AnswerChoiceText = "Green" });
			q1.AnswerChoices.Add(new SurveyQuestionAnswerChoiceModel { AnswerChoiceID = 3, AnswerChoiceNum = "c", AnswerChoiceText = "Blue", NextQuestionID = -1 });
			q1.AnswerChoices.Add(new SurveyQuestionAnswerChoiceModel { AnswerChoiceID = 4, AnswerChoiceNum = "d", AnswerChoiceText = "Yellow" });

			var q2 = new SurveyQuestionModel { QuestionID = 2, QuestionNum = "1a", QuestionText = "What shade of your favorite color do you prefer?", QuestionHelpText = "" };

			q2.AnswerChoices.Add(new SurveyQuestionAnswerChoiceModel { AnswerChoiceID = 5, AnswerChoiceNum = "a", AnswerChoiceText = "Light" });
			q2.AnswerChoices.Add(new SurveyQuestionAnswerChoiceModel { AnswerChoiceID = 6, AnswerChoiceNum = "b", AnswerChoiceText = "Medium" });
			q2.AnswerChoices.Add(new SurveyQuestionAnswerChoiceModel { AnswerChoiceID = 7, AnswerChoiceNum = "c", AnswerChoiceText = "Dark" });

			sm.Questions.Add(q1);
			sm.Questions.Add(q2);

			var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sm);

			System.Diagnostics.Debug.WriteLine(json);
		}

		[TestMethod]
		public void GetSurveyJSON()
		{
			using (var db = new WeCountContext("WeCountContext"))
			{
				db.Surveys.Count();
			}

		}
	}
}
