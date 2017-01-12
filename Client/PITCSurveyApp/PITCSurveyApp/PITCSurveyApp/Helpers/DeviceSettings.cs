using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PITCSurveyApp.Helpers
{
    static class DeviceSettings
    {
        private const string DeviceIdKey = "deviceid";

        // HockeyApp App IDs, we have one for each supported platform
#if WINDOWS_UWP
        public static string HockeyAppId = "49625860a7584af69e3886038eeec213";
#elif __ANDROID__
        public static string HockeyAppId = "138e6068e68d49f6a8bcda8bc5ec1150";
#elif __IOS__
        public static string HockeyAppId = "b086636c4eb243489a5261339c32aa56";
#endif

        private static ISettings AppSettings => CrossSettings.Current;

        public static string DeviceId
        {
            get
            {
                string did = AppSettings.GetValueOrDefault(DeviceIdKey, default(string));

                if (did == null)
                {
                    // Set initial value and save
                    did = System.Guid.NewGuid().ToString();

                    DeviceId = did;
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
