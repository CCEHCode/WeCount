using System;
using System.Diagnostics;
using PITCSurveyApp.Services;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
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
