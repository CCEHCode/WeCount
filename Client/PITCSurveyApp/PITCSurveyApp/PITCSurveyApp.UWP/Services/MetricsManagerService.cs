using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.HockeyApp;

using PITCSurveyApp.Extensions;

[assembly: Xamarin.Forms.Dependency(typeof(PITCSurveyApp.UWP.MetricsManagerService))]
namespace PITCSurveyApp.UWP
{
    class MetricsManagerService : IMetricsManagerService
    {
        public void TrackEvent(string eventName)
        {
            HockeyClient.Current.TrackEvent(eventName);
        }
    }
}