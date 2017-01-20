using System;
using System.Collections.Generic;
using Microsoft.HockeyApp;
using PITCSurveyApp.Services;

[assembly: Xamarin.Forms.Dependency(typeof(PITCSurveyApp.UWP.WindowsMetricsManagerService))]

namespace PITCSurveyApp.UWP
{
    class WindowsMetricsManagerService : IMetricsManagerService
    {
        public void TrackEvent(string eventName)
        {
            HockeyClient.Current.TrackEvent(eventName);
        }

        public void TrackException(string eventName, Exception ex)
        {
            HockeyClient.Current.TrackException(ex);
        }

        public void TrackLatency(string eventName, TimeSpan latency)
        {
            var measurements = new Dictionary<string, double>
            {
                { "latency", latency.TotalMilliseconds },
            };

            TrackEvent(eventName, null, measurements);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties, Dictionary<string, double> measurements)
        {
            HockeyClient.Current.TrackEvent(eventName, properties, measurements);
        }
    }
}