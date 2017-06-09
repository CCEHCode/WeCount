using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// Helpers for cross-platform operations.
    /// </summary>
    public static class CrossHelper
    {
        /// <summary>
        /// Returns the proper location for hardcoded resource images loaded from local files.
        /// This is needed since Device.OnPlatform doesn't support detection of the "Windows" OS
        /// for scenarios that use Windows 10/UWP or Windows 8.1.
        /// 
        /// This helper function assumes that images are all named the same across platforms,
        /// and are located in the following folders:
        ///  - iOS: BundleResource in Resources folder
        ///  - Android: AndroidResource in Resources/Drawable folder
        ///  - Windows: Content in Assets folder
        /// </summary>
        /// <param name="filename">The filename of the image we need to load from file.</param>
        /// <returns>The full name image path and file name.</returns>
        public static string GetOSFullImagePath(string filename)
        {
            switch (Device.OS)
            {
                case TargetPlatform.iOS:
                    return filename;
                case TargetPlatform.Android:
                    return filename;
                case TargetPlatform.Windows:
                    return "Assets/" + filename;
                case TargetPlatform.WinPhone:
                    // Not supported in this project, but just in case
                    return "Assets/" + filename;
                case TargetPlatform.Other:
                    // Not supported in this project, but just in case
                    return filename;
                default:
                    return filename;
            }
        }
    }
}
