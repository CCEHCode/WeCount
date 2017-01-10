using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyLib.Models;
using PITCSurveyLib;

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
    }
}
