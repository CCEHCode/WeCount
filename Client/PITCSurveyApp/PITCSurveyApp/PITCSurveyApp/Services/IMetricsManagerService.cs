using System;
using System.Collections.Generic;

namespace PITCSurveyApp.Services
{
    public interface IMetricsManagerService
    {
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, Dictionary<string, string> properties, Dictionary<string, double> measurements);
        void TrackException(string userloginfailed, Exception exception);
        void TrackLatency(string eventName, TimeSpan latency);
    }
}
