using System;
using System.Collections.Generic;

namespace PITCSurveyApp.Services
{
    /// <summary>
    /// An interface for required metrics tracking operations.
    /// </summary>
    public interface IMetricsManagerService
    {
        /// <summary>
        /// Track an event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        void TrackEvent(string eventName);

        /// <summary>
        /// Track an event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="properties">The related properties.</param>
        /// <param name="measurements">The related measurements.</param>
        void TrackEvent(string eventName, Dictionary<string, string> properties, Dictionary<string, double> measurements);

        /// <summary>
        /// Track an exception.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="exception">The exception.</param>
        void TrackException(string eventName, Exception exception);

        /// <summary>
        /// Track a latency metric.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="latency">The latency associated with the event.</param>
        void TrackLatency(string eventName, TimeSpan latency);
    }
}
