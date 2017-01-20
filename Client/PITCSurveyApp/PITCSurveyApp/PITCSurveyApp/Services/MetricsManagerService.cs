using Xamarin.Forms;

namespace PITCSurveyApp.Services
{
    /// <summary>
    /// A singleton instance manager for <see cref="IMetricsManagerService"/>.  
    /// </summary>
    static class MetricsManagerService
    {
        /// <summary>
        /// The singleton instance of <see cref="IMetricsManagerService"/>. 
        /// </summary>
        public static readonly IMetricsManagerService Instance = DependencyService.Get<IMetricsManagerService>();
    }
}
