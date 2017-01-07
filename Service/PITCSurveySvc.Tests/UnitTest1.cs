using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PITCSurveyLib;
using PITCSurveyLib.Models;

namespace PITCSurveySvc.Tests
{
	[TestClass]
	public class APITests
	{
		private const string APIURL = "https://pitcsurveyapi.azurewebsites.net";

		private APIHelper API;

		[TestInitialize]
		public void Init()
		{
			var Creds = new Microsoft.Rest.BasicAuthenticationCredentials() { UserName = "", Password = "" };

			API = new APIHelper();
		}

		[TestMethod]
		public void TestSubmitResponse_Null()
		{
			try
			{
				var result = API.SubmitSurveyResponse(null);

				Assert.IsFalse(result, "Null submission should be rejected.");
			}
			catch (APIException ex)
			{
				System.Diagnostics.Trace.WriteLine("Null response error: " + ex.ToString());
				//Assert.Fail(ex.Message);
			}

		}

		[TestMethod]
		public void TestSubmitResponse_New()
		{
			var Resp = new SurveyResponseModel()
			{
				SurveyID = 1,
				ResponseIdentifier = Guid.NewGuid(),
			};

			try
			{
				var result = API.SubmitSurveyResponse(Resp);

				Assert.IsTrue(result, "This should have worked.");
			}
			catch (APIException ex)
			{
				//Assert.Fail("Response submission failed: " + ex.Message);
				Assert.Fail(ex.Message);
			}

		}

		[TestMethod]
		public void TestSubmitResponse_Duplicate()
		{
			var Resp = new SurveyResponseModel()
			{
				SurveyID = 1,
				ResponseIdentifier = Guid.Empty,
			};

			try
			{
				var result = API.SubmitSurveyResponse(Resp);

				Assert.IsFalse(result, "Duplicate submission should have failed.");
			}
			catch (APIException ex)
			{
				System.Diagnostics.Trace.WriteLine("Duplicate response error: " + ex.ToString());
				Assert.Fail(ex.Message);
			}

		}
	}
}
