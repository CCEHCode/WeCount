using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Extensions;
using PITCSurveyApp.Helpers;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Services
{
    public static class SurveyCloudService
    {
        private const string AzureMobileAppUrl = "https://appname.azurewebsites.net";

        public static MobileServiceClient ApiClient;

        static SurveyCloudService()
        {
            ApiClient = new MobileServiceClient(AzureMobileAppUrl);
        }

        public static async Task<SurveyModel> GetSurveyAsync(int id = 1)
        {
            var parameters = new Dictionary<string, string>
            {
                {"id", id.ToString()},
            };

            try
            {
                return await ApiClient.InvokeApiAsync<SurveyModel>("Surveys", HttpMethod.Get, parameters);
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("GetSurveyFailed", ex);
                return null;
            }
        }

        public static Task SubmitSurveyResponseAsync(SurveyResponseModel response)
        {
            var parameters = new Dictionary<string, string>
            {
                {"DeviceId", UserSettings.VolunteerId},
            };

            return ApiClient.InvokeApiAsync("SurveyResponses", JObject.FromObject(response), HttpMethod.Post, parameters);
        }

        public static async Task<VolunteerModel> GetVolunteerAsync()
        {
            try
            {
				var parameters = new Dictionary<string, string>
			    {
				    {"DeviceId", UserSettings.VolunteerId},
			    };

				 return await ApiClient.InvokeApiAsync<VolunteerModel>("Volunteers", HttpMethod.Get, parameters);
            }
            catch (MobileServiceInvalidOperationException ex)
                when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new VolunteerModel();
            }
        }

        public static Task SaveVolunteerAsync(VolunteerModel volunteer)
        {
			var parameters = new Dictionary<string, string>
			{
				{"DeviceId", UserSettings.VolunteerId},
			};

			return ApiClient.InvokeApiAsync("Volunteers", JObject.FromObject(volunteer), HttpMethod.Put, parameters);
        }
    }
}
