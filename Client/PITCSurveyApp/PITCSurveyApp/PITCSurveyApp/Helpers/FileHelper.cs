using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    class FileHelper //: IFileHelper
    {
        private readonly IFileHelper _fileHelper = DependencyService.Get<IFileHelper>();

		// ATY: Were these intentionally not marked async?

        public async Task<bool> ExistsAsync(string filename)
        {
			//return _fileHelper.ExistsAsync(filename);
			//var file = _fs.GetFileFromPathAsync(filename).Result;
			//return Task.FromResult(file != null);
			return System.IO.File.Exists(filename);
        }

        public async Task<DateTime> LastModifiedAsync(string filename)
        {
			//return _fileHelper.LastModifiedAsync(filename);
			return System.IO.File.GetLastWriteTime(filename);
		}

		public async Task WriteTextAsync(string filename, string text)
        {
			//return _fileHelper.WriteTextAsync(filename, text);
			System.IO.File.WriteAllText(filename, text);
			return;
        }

        public async Task<string> ReadTextAsync(string filename)
        {
			//return _fileHelper.ReadTextAsync(filename);
			return System.IO.File.ReadAllText(filename);
        }

        public Task<IEnumerable<string>> GetFilesAsync()
        {
			//return await _fileHelper.GetFilesAsync();
			return Task.FromResult(System.IO.Directory.GetFiles(GetDocsPath()).AsEnumerable());
        }

        public async Task DeleteAsync(string filename)
        {
			//return _fileHelper.DeleteAsync(filename);
			System.IO.File.Delete(filename);
        }

		// Moved from extensions class

		public async Task SaveAsync<T>(string filename, T item)
		{
			var jsonItem = JObject.FromObject(item);
			var jsonText = jsonItem.ToString(Formatting.None);
			await WriteTextAsync(filename, jsonText);
		}

		public async Task<T> LoadAsync<T>(string filename)
		{
			var jsonText = await ReadTextAsync(filename);
			var jsonItem = JObject.Parse(jsonText);
			return jsonItem.ToObject<T>();
		}

		private string GetDocsPath()
		{
			return _fileHelper.GetDocsPath().Result;
		}

	}
}
