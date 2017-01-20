using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Helpers;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Services
{
    /// <summary>
    /// Collection of helper methods for accessing the survey cloud service.
    /// </summary>
    public static class SurveyCloudService
    {
        private const string AzureMobileAppUrl = "https://appname.azurewebsites.net";

        /// <summary>
        /// The API client.
        /// </summary>
        public static readonly MobileServiceClient ApiClient = new MobileServiceClient(AzureMobileAppUrl);

        /// <summary>
        /// Gets the survey with the given identifier.
        /// </summary>
        /// <param name="id">The survey identifier.</param>
        /// <returns>
        /// A task to await the survey surveyResponse, returning the survey surveyResponse
        /// or <code>null</code> if any exception occurs.
        /// </returns>
        public static async Task<SurveyModel> GetSurveyAsync(int id)
        {
            var parameters = new Dictionary<string, string>
            {
                {"id", id.ToString()},
            };

            try
            {
                using (new LatencyMetric("GetSurvey"))
                {
                    return await ApiClient.InvokeApiAsync<SurveyModel>("Surveys", HttpMethod.Get, parameters);
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("GetSurveyFailed", ex);
                return null;
            }
        }

        /// <summary>
        /// Submits a survey response to the service.
        /// </summary>
        /// <param name="surveyResponse">The survey response</param>
        /// <returns>
        /// A task to await the submission. 
        /// </returns>
        public static Task SubmitSurveyResponseAsync(SurveyResponseModel surveyResponse)
        {
            var parameters = new Dictionary<string, string>
            {
                {"deviceId", UserSettings.VolunteerId},
            };

            using (new LatencyMetric("SubmitSurveyResponse"))
            {
                return ApiClient.InvokeApiAsync("SurveyResponses", JObject.FromObject(surveyResponse), HttpMethod.Post, parameters);
            }
        }

        /// <summary>
        /// Gets the current volunteer profile.
        /// </summary>
        /// <returns>
        /// A task to await the volunteer profile, or a new instance of 
        /// <see cref="VolunteerModel"/> if an existing record cannot be found.
        /// </returns>
        public static async Task<VolunteerModel> GetVolunteerAsync()
        {
            try
            {
				var parameters = new Dictionary<string, string>
			    {
				    {"deviceId", UserSettings.VolunteerId},
			    };

                using (new LatencyMetric("GetVolunteer"))
                {
                    return await ApiClient.InvokeApiAsync<VolunteerModel>("Volunteers", HttpMethod.Get, parameters);
                }
            }
            catch (MobileServiceInvalidOperationException ex)
                when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new VolunteerModel();
            }
        }

        /// <summary>
        /// Saves the volunteer profile.
        /// </summary>
        /// <param name="volunteer">The volunteer profile.</param>
        /// <returns>
        /// A task to await the save operation.
        /// </returns>
        public static Task SaveVolunteerAsync(VolunteerModel volunteer)
        {
			var parameters = new Dictionary<string, string>
			{
				{"deviceId", UserSettings.VolunteerId},
			};

            using (new LatencyMetric("SaveVolunteer"))
            {
                return ApiClient.InvokeApiAsync("Volunteers", JObject.FromObject(volunteer), HttpMethod.Put, parameters);
            }
        }

        /// <summary>
        /// Gets the data for the contact information page.
        /// </summary>
        /// <param name="surveyId">
        /// The survey identifier linked to the contact information.
        /// </param>
        /// <returns>
        /// A task to await the contact information.
        /// </returns>
		public static async Task<ContactInfoModel> GetContactInfoAsync(int surveyId)
		{
			var parameters = new Dictionary<string, string>
			{
				{ "surveyId", surveyId.ToString() },
				{ "deviceId", UserSettings.VolunteerId},
			};

			using (new LatencyMetric("GetContactInfo"))
			{
				return await ApiClient.InvokeApiAsync<ContactInfoModel>("ContactInfo", HttpMethod.Get, parameters);
			}
		}
	}
}
