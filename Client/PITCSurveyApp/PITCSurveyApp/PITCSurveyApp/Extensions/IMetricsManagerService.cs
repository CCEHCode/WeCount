using System;
using System.Collections.Generic;
using System.Text;

namespace PITCSurveyApp.Extensions
{
    public interface IMetricsManagerService
    {
        void TrackEvent(string eventName);
    }
}
