using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    class FileHelper : IFileHelper
    {
        private readonly IFileHelper _fileHelper = DependencyService.Get<IFileHelper>();

		// ATY: Were these intentionally not marked async?

        public async Task<bool> ExistsAsync(string filename)
        {
			return await _fileHelper.ExistsAsync(filename);
        }

        public async Task<DateTime> LastModifiedAsync(string filename)
        {
			return await _fileHelper.LastModifiedAsync(filename);
		}

		public async Task WriteTextAsync(string filename, string text)
        {
			await _fileHelper.WriteTextAsync(filename, text);
        }

        public async Task<string> ReadTextAsync(string filename)
        {
			return await _fileHelper.ReadTextAsync(filename);
        }

        public async Task<IEnumerable<string>> GetFilesAsync()
        {
			return await _fileHelper.GetFilesAsync();
        }

        public async Task DeleteAsync(string filename)
        {
			await _fileHelper.DeleteAsync(filename);
        }
	}
}
