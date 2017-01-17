using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    class FileHelper : IFileHelper
    {
        private readonly IFileHelper _fileHelper = DependencyService.Get<IFileHelper>();

        public Task<bool> ExistsAsync(string filename)
        {
			return _fileHelper.ExistsAsync(filename);
        }

        public Task<DateTime> LastModifiedAsync(string filename)
        {
			return _fileHelper.LastModifiedAsync(filename);
		}

		public Task WriteTextAsync(string filename, string text)
        {
			return _fileHelper.WriteTextAsync(filename, text);
        }

        public Task<string> ReadTextAsync(string filename)
        {
			return _fileHelper.ReadTextAsync(filename);
        }

        public Task<IEnumerable<string>> GetFilesAsync()
        {
			return _fileHelper.GetFilesAsync();
        }

        public Task DeleteAsync(string filename)
        {
			return _fileHelper.DeleteAsync(filename);
        }
	}
}
