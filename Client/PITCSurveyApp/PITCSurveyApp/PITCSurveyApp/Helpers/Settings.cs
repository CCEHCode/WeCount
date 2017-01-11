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
        private const string AuthTokenKey = "authtoken";
        private const string UserIdKey = "userid";

        // HockeyApp App IDs, we have one for each supported platform
#if WINDOWS_UWP
        public static string HockeyAppId = "hockeyAppId";
#elif __ANDROID__
        public static string HockeyAppId = "hockeyAppId";
#elif __IOS__
        public static string HockeyAppId = "hockeyAppId";
#endif

        private static ISettings AppSettings => CrossSettings.Current;

        public static string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault(UserIdKey, default(string));
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserIdKey, value);
            }
        }

        public static string AuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(AuthTokenKey, default(string));
            }
            set
            {
                AppSettings.AddOrUpdateValue(AuthTokenKey, value);
            }
        }

        public static bool Initializing { get; set; }

        public static bool IsLoggedIn => !Initializing && !string.IsNullOrWhiteSpace(AuthToken);
    }
}