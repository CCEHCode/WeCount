using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using HockeyApp;

using PITCSurveyApp.Extensions;

[assembly: Xamarin.Forms.Dependency(typeof(PITCSurveyApp.iOS.MetricsManagerService))]
namespace PITCSurveyApp.iOS
{
    class MetricsManagerService : IMetricsManagerService
    {
        public void TrackEvent(string eventName)
        {
            MetricsManager.TrackEvent(eventName);
        }
    }
}