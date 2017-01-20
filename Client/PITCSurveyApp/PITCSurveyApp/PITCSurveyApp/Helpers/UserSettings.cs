using System;
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
        private const string VolunteerIdKey = "deviceid";

        // HockeyApp App IDs, we have one for each supported platform
#if WINDOWS_UWP
        public const string HockeyAppId = "hockeyAppId";
#elif __ANDROID__
        public const string HockeyAppId = "hockeyAppId";
#elif __IOS__
        public const string HockeyAppId = "hockeyAppId";
#endif

        private static ISettings AppSettings => CrossSettings.Current;

        /// <summary>
        /// The cached user ID received from login.
        /// </summary>
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

        /// <summary>
        /// The cached authentication token received from login.
        /// </summary>
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

        /// <summary>
        /// Signals that the application is still attempting to refresh the user token.
        /// </summary>
        public static bool IsRefreshingAuthToken { get; set; }

        /// <summary>
        /// Signals that the user is logged in.
        /// </summary>
        public static bool IsLoggedIn => !IsRefreshingAuthToken && App.Authenticator.User != null;

        /// <summary>
        /// The active volunteer model.
        /// </summary>
        public static VolunteerModel Volunteer { get; set; } = new VolunteerModel();

        /// <summary>
        /// A unique volunteer identifier used to identify unauthenticated users to the service.
        /// </summary>
        /// <remarks>
        /// The volunteer identifier persists until an authenticated user logs out. 
        /// After logging out, a new volunteer identifier will be generated and cached.
        /// </remarks>
        public static string VolunteerId
        {
            get
            {
                var volunteerId = AppSettings.GetValueOrDefault(VolunteerIdKey, default(string));
                if (string.IsNullOrEmpty(volunteerId))
                {
                    // Set initial value and save
                    volunteerId = Guid.NewGuid().ToString();
                    VolunteerId = volunteerId;
                }

                return volunteerId;
            }
            set
            {
                AppSettings.AddOrUpdateValue(VolunteerIdKey, value);
            }
        }
    }
}