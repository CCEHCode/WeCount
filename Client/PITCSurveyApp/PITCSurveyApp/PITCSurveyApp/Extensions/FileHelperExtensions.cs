using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Helpers;

namespace PITCSurveyApp.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IFileHelper"/>. 
    /// </summary>
	static class FileHelperExtensions
    {
        /// <summary>
        /// Save an item to the given filename, serialized as JSON.
        /// </summary>
        /// <typeparam name="T">Type of item to save.</typeparam>
        /// <param name="fileHelper">The file helper.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="item">The item to save.</param>
        /// <returns>A task to await the asynchronous save operation.</returns>
		public static Task SaveAsync<T>(this IFileHelper fileHelper, string filename, T item)
        {
            var jsonItem = JObject.FromObject(item);
            var jsonText = jsonItem.ToString(Formatting.None);
            return fileHelper.WriteTextAsync(filename, jsonText);
        }

        /// <summary>
        /// Load an item serialized as JSON from the given filename.
        /// </summary>
        /// <typeparam name="T">Type of item to load.</typeparam>
        /// <param name="fileHelper">The file helper.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>A task to await the asynchronous load operation, returning the loaded item.</returns>
		public static async Task<T> LoadAsync<T>(this IFileHelper fileHelper, string filename)
        {
            var jsonText = await fileHelper.ReadTextAsync(filename);
            var jsonItem = JObject.Parse(jsonText);
            return jsonItem.ToObject<T>();
        }
    }
}
