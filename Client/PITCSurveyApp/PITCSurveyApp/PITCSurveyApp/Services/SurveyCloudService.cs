using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

using PITCSurveyLib.Models;
using PITCSurveyLib;

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

        public static async Task<SurveyModel> GetLatestSurvey(int id = 1)
        {
			try
			{
				return await APIHelper.GetSurveyByIDAsync(id);
			}
			catch (Exception)
			{
				// TODO: Return error to show to user?
				return null;
			}
        }

    }
}
