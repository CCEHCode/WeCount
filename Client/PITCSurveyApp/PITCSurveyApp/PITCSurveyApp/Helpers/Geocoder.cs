using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    static class Geocoder
    {
        private const string BingMapsKey = "bingMapsKey";

        public static async Task<AddressResult> ReverseGeocode(double latitude, double longitude)
        {
            var queryString = $"http://dev.virtualearth.net/REST/v1/Locations/{latitude},{longitude}?key={BingMapsKey}";
            var webRequest = WebRequest.Create(queryString);
            try
            {
                using (new LatencyMetric("ReverseGeocode"))
                using (var response = await webRequest.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                using (var streamReader = new StreamReader(stream))
                {
                    var responseText = await streamReader.ReadToEndAsync();
                    var responseJson = JObject.Parse(responseText);
                    var addressJson = (JObject)responseJson["resourceSets"]?.First?["resources"]?.First?["address"];
                    return addressJson?.ToObject<AddressResult>();
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("ReverseGeocodeFailed", ex);
                return null;
            }
        }
    }
}
