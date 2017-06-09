using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PITCSurveyLib;
using PITCSurveyLib.Models;
using PITCSurveyEntities.Entities;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PITCSurveySvc.Tests
{
    [TestClass]
    public class APITests
    {
        private const string APIURL = "https://appname.azurewebsites.net";
        private readonly string _DeviceId = "07678abf-857c-4266-a1e1-4552c97db13c";

        private MobileServiceClient ApiClient = new MobileServiceClient(APIURL);

        private PITCSurveyContext db = new PITCSurveyContext();

        [TestMethod]
        public void TestSubmitResponse_Null()
        {
            try
            {
                SubmitSurveyResponse(null, _DeviceId);

                Assert.Fail("Null submission should be rejected.");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Null response error: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestSubmitResponse_NoAuthNoDeviceId()
        {
            try
            {
                var Resp = TestHelpers.GenerateRandomResponseForSurvey(db.Surveys.Where(s => s.ID == 1).Single());

                SubmitSurveyResponse(Resp, null);

                Assert.Fail("Unauthenticated, null DeviceId submission should be rejected.");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Unauth no DeviceId response error: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestSubmitResponse_New()
        {
            var Resp = TestHelpers.GenerateRandomResponseForSurvey(db.Surveys.Where(s => s.ID == 1).Single());

            SubmitSurveyResponse(Resp, _DeviceId);
        }

        /*
        [TestMethod]
        public void TestSubmitResponse_MakeABunch()
        {
            for (int i = 0; i < 20; i++)
            {
                var Resp = TestHelpers.GenerateRandomResponseForSurvey(db.Surveys.Where(s => s.ID == 1).Single());

                var result = SubmitSurveyResponse(Resp, DeviceId);
            }
        }
        */

        [TestMethod]
        public void TestSubmitResponse_Duplicate()
        {
            var Resp = TestHelpers.GenerateRandomResponseForSurvey(db.Surveys.Where(s => s.ID == 1).Single());

            Resp.ResponseIdentifier = Guid.Empty;

            SubmitSurveyResponse(Resp, _DeviceId);
        }

        private void SubmitSurveyResponse(SurveyResponseModel Response, String DeviceId)
        {
            var parameters = new Dictionary<string, string>
            {
                {"DeviceId", DeviceId },
            };

            ApiClient.InvokeApiAsync("SurveyResponses", JObject.FromObject(Response), HttpMethod.Post, parameters).Wait();
        }
    }
}
