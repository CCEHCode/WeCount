using System;
using System.Collections.Generic;
using HockeyApp;
using PITCSurveyApp.Services;

[assembly: Xamarin.Forms.Dependency(typeof(PITCSurveyApp.iOS.MetricsManagerService))]

namespace PITCSurveyApp.iOS
{
    class MetricsManagerService : IMetricsManagerService
    {
        public void TrackEvent(string eventName)
        {
            MetricsManager.TrackEvent(eventName);
        }
        public void TrackException(string eventName, Exception ex)
        {
            var properties = new Dictionary<string, string>
            {
                {"error", ex.Message},
            };

            TrackEvent(eventName, properties, null);
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
            MetricsManager.TrackEvent(eventName, properties, measurements);
        }
    }
}