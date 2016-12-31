using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

using PITCSurveyLib.Models;

namespace PITCSurveyApp.Lib.Services
{
    public static class SurveyCloudService
    {
        private const string AzureMobileAppUrl = "https://appname.azurewebsites.net";

        private static MobileServiceClient ApiClient;

        static SurveyCloudService()
        {
            ApiClient = new MobileServiceClient(AzureMobileAppUrl);
        }

        public static async Task<SurveyModel> GetLatestSurvey()
        {
            var parameters = new Dictionary<string, string>{{"id", "1"}};

            try
            {
                var result = await ApiClient.InvokeApiAsync<SurveyModel>("Surveys", System.Net.Http.HttpMethod.Get, parameters);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
