using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PITCSurveyApp.Helpers;
using Xamarin.Forms;
using System.Linq;

[assembly: Dependency(typeof(PITCSurveyApp.iOS.Helpers.FileHelper))]

namespace PITCSurveyApp.iOS.Helpers
{
    class FileHelper : IFileHelper
    {
        public Task<bool> ExistsAsync(string filename)
        {
			return Task.FromResult(File.Exists(filename));
		}

		public Task<DateTime> LastModifiedAsync(string filename)
        {
			return Task.FromResult(File.GetLastWriteTime(filename));
		}

		public async Task WriteTextAsync(string filename, string text)
        {
			var filepath = GetFilePath(filename);
			using (var streamWriter = File.CreateText(filepath))
			{
				await streamWriter.WriteAsync(text);
			}
		}

		public async Task<string> ReadTextAsync(string filename)
        {
            var filepath = GetFilePath(filename);
            using (StreamReader reader = File.OpenText(filepath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            var files = Directory.GetFiles(GetDocsPath());
			return await Task.FromResult<IEnumerable<string>>(files.AsEnumerable());
        }

        public async Task DeleteAsync(string filename)
        {
            await Task.Run(() => File.Delete(GetFilePath(filename)));
			return;
        }

        // Private methods.
        string GetFilePath(string filename)
        {
            return Path.Combine(GetDocsPath(), filename);
        }

        string GetDocsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }
}
 