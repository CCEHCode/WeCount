using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Helpers;

namespace PITCSurveyApp.Extensions
{
	static class FileHelperExtensions
    {
		public static Task SaveAsync<T>(this IFileHelper fileHelper, string filename, T item)
        {
            var jsonItem = JObject.FromObject(item);
            var jsonText = jsonItem.ToString(Formatting.None);
            return fileHelper.WriteTextAsync(filename, jsonText);
        }

		public static async Task<T> LoadAsync<T>(this IFileHelper fileHelper, string filename)
        {
            var jsonText = await fileHelper.ReadTextAsync(filename);
            var jsonItem = JObject.Parse(jsonText);
            return jsonItem.ToObject<T>();
        }
    }
}
