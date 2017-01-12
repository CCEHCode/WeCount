using PITCSurveyLib.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class UserSettings
    {
        private const string AuthTokenKey = "authtoken";
        private const string UserIdKey = "userid";
        private const string DeviceIdKey = "deviceid";

        // HockeyApp App IDs, we have one for each supported platform
#if WINDOWS_UWP
        public const string HockeyAppId = "hockeyAppId";
#elif __ANDROID__
        public const string HockeyAppId = "hockeyAppId";
#elif __IOS__
        public const string HockeyAppId = "hockeyAppId";
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

        public static VolunteerModel Volunteer { get; set; } = new VolunteerModel();

        public static string VolunteerId
        {
            get
            {
                string did = AppSettings.GetValueOrDefault(DeviceIdKey, default(string));

                if (did == null)
                {
                    // Set initial value and save
                    did = System.Guid.NewGuid().ToString();

                    VolunteerId = did;
                }

                return did;
            }
            set
            {
                AppSettings.AddOrUpdateValue(DeviceIdKey, value);
            }
        }
    }
}