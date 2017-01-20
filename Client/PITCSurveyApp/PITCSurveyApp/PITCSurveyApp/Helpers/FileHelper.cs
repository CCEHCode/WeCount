using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// A cross-platform helper for accessing the local filesystem.
    /// </summary>
    class FileHelper : IFileHelper
    {
        private readonly IFileHelper _fileHelper = DependencyService.Get<IFileHelper>();

        /// <summary>
        /// Checks if a file with the given filename exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// A task to await the exists operation, returning <code>true</code>
        /// if the file exists, otherwise <code>false</code>.
        /// </returns>
        public Task<bool> ExistsAsync(string filename)
        {
			return _fileHelper.ExistsAsync(filename);
        }

        /// <summary>
        /// Gets the last modified time from the file with the given name.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// A task to await the last modified operation, returning the last modified date.
        /// </returns>
        public Task<DateTime> LastModifiedAsync(string filename)
        {
			return _fileHelper.LastModifiedAsync(filename);
		}

        /// <summary>
        /// Writes text to the given filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="text">The text to write.</param>
        /// <returns>A task to await the write operation.</returns>
		public Task WriteTextAsync(string filename, string text)
        {
			return _fileHelper.WriteTextAsync(filename, text);
        }

        /// <summary>
        /// Reads text from the given filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// A task to await the read operation, returning the full text of the file.
        /// </returns>
        public Task<string> ReadTextAsync(string filename)
        {
			return _fileHelper.ReadTextAsync(filename);
        }

        /// <summary>
        /// Enumerates all files in the filesystem.
        /// </summary>
        /// <returns>
        /// A task to await the enumerate operation, returning the collection of files.
        /// </returns>
        public Task<IEnumerable<string>> GetFilesAsync()
        {
			return _fileHelper.GetFilesAsync();
        }

        /// <summary>
        /// Deletes a file with the given filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>A task to await the delete operation.</returns>
        public Task DeleteAsync(string filename)
        {
			return _fileHelper.DeleteAsync(filename);
        }
	}
}
