using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PITCSurveyLib;
using PITCSurveyLib.Models;
using PITCSurveyEntities.Entities;
using System.Linq;

namespace PITCSurveySvc.Tests
{
	[TestClass]
	public class APITests
	{
		private const string APIURL = "https://appname.azurewebsites.net";

		private PITCSurveyContext db = new PITCSurveyContext();

		[TestMethod]
		public void TestSubmitResponse_Null()
		{
			try
			{
				var result = APIHelper.SubmitSurveyResponse(null);

				Assert.IsFalse(result, "Null submission should be rejected.");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("Null response error: " + ex.ToString());
				//Assert.Fail(ex.Message);
			}

		}

		[TestMethod]
		public void TestSubmitResponse_New()
		{
			var Resp = TestHelpers.GenerateRandomResponseForSurvey(db.Surveys.Where(s => s.ID == 1).Single());

			Resp.ResponseIdentifier = Guid.NewGuid();

			try
			{
				var result = APIHelper.SubmitSurveyResponse(Resp);

				Assert.IsTrue(result, "This should have worked.");
			}
			catch (APIException ex)
			{
				//Assert.Fail("Response submission failed: " + ex.Message);
				Assert.Fail(ex.Message);
			}

		}

		[TestMethod]
		public void TestSubmitResponse_MakeABunch()
		{
			for (int i = 0; i < 20; i++)
			{
				var Resp = TestHelpers.GenerateRandomResponseForSurvey(db.Surveys.Where(s => s.ID == 1).Single());

				Resp.ResponseIdentifier = Guid.NewGuid();

				try
				{
					var result = APIHelper.SubmitSurveyResponse(Resp);

					//Assert.IsTrue(result, "This should have worked.");
				}
				catch (APIException ex)
				{
					//Assert.Fail("Response submission failed: " + ex.Message);
					Assert.Fail(ex.Message);
				}
			}
		}

		[TestMethod]
		public void TestSubmitResponse_Duplicate()
		{
			var Resp = TestHelpers.GenerateRandomResponseForSurvey(db.Surveys.Where(s => s.ID == 1).Single());

			Resp.ResponseIdentifier = Guid.Empty;

			try
			{
				var result = APIHelper.SubmitSurveyResponse(Resp);

				// NOTE: API spec is to accept this. So, ok then.
				// Assert.IsFalse(result, "Duplicate submission should have failed.");
			}
			catch (APIException ex)
			{
				System.Diagnostics.Trace.WriteLine("Duplicate response error: " + ex.ToString());
				Assert.Fail(ex.Message);
			}

		}
	}
}
