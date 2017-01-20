using System;
using System.Diagnostics;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// A struct for measuring latency of operations.
    /// </summary>
    /// <remarks>
    /// Using struct here to prevent impact on the GC. Pass by value is fine as
    /// the only behavior occurs on dispose. Multiple calls to dispose will
    /// result in multiple latency metrics being tracked, so it is suggested
    /// to use this helper in the context of a <code>using</code> block, e.g.:
    /// 
    /// <code>
    /// using (new LatencyMetric("MyMetric"))
    /// {
    ///    /* Do stuff */
    /// }
    /// </code>
    /// 
    /// </remarks>
    struct LatencyMetric : IDisposable
    {
        private static readonly Stopwatch s_stopwatch = Stopwatch.StartNew();

        private readonly string _eventName;
        private readonly long _startTicks;

        public LatencyMetric(string eventName)
        {
            _eventName = eventName;
            _startTicks = s_stopwatch.ElapsedTicks;
        }

        public void Dispose()
        {
            var metricsManager = DependencyService.Get<IMetricsManagerService>();
            metricsManager.TrackLatency(_eventName, TimeSpan.FromTicks(s_stopwatch.ElapsedTicks - _startTicks));
        }
    }
}
