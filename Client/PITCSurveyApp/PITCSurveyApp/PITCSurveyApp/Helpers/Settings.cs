using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        // HockeyApp App IDs, we have one for each supported platform
#if WINDOWS_UWP
        public static string HockeyAppId = "hockeyAppId";
#elif __ANDROID__
        public static string HockeyAppId = "hockeyAppId";
#elif __IOS__
        public static string HockeyAppId = "hockeyAppId";
#endif

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static string AuthToken { get; set; }

        public static bool IsLoggedIn => !string.IsNullOrWhiteSpace(AuthToken);
    }
}