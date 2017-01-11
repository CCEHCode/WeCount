using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using PITCSurveyLib.Models;

namespace PITCSurveyApp.Services
{
    public static class SurveyCloudService
    {
        private const string AzureMobileAppUrl = "https://appname.azurewebsites.net";

		// TODO: Move this into APIHelper for consistency?
        public static MobileServiceClient ApiClient;

        static SurveyCloudService()
        {
            ApiClient = new MobileServiceClient(AzureMobileAppUrl);
        }

        public static async Task<SurveyModel> GetSurveyAsync(int id = 1)
        {
            var parameters = new Dictionary<string, string>
            {
                { "id", "1" },
            };

            try
            {
                return await ApiClient.InvokeApiAsync<SurveyModel>("Surveys", System.Net.Http.HttpMethod.Get, parameters);
            }
            catch (Exception ex)
            {
                // TODO: log exception
                return null;
            }
        }

        public static Task SubmitSurveyResponseAsync(SurveyResponseModel response)
        {
            return ApiClient.InvokeApiAsync("SurveyResponses", JObject.FromObject(response));
        }
    }
}
