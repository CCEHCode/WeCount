using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using PITCSurveyApp.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(PITCSurveyApp.UWP.Helpers.WindowsFileHelper))]

namespace PITCSurveyApp.UWP.Helpers
{
    class WindowsFileHelper : IFileHelper
    {
        public async Task<bool> ExistsAsync(string filename)
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                await localFolder.GetFileAsync(filename);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            return true;
        }

        public async Task<DateTime> LastModifiedAsync(string filename)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var storageFile = await localFolder.GetFileAsync(filename);
            var properties = await storageFile.GetBasicPropertiesAsync();
            return properties.DateModified.DateTime;
        }

        public async Task WriteTextAsync(string filename, string text)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var storageFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(storageFile, text);
        }

        public async Task<string> ReadTextAsync(string filename)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var storageFile = await localFolder.GetFileAsync(filename);
            return await FileIO.ReadTextAsync(storageFile);
        }

        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var files = await localFolder.GetFilesAsync();
            return files.Select(f => f.Name);
        }

        public async Task DeleteAsync(string filename)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var storageFile = await localFolder.GetFileAsync(filename);
            await storageFile.DeleteAsync();
        }
    }
}
