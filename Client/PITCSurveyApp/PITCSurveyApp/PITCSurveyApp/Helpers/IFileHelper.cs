using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// An interface for required filesystem operations.
    /// </summary>
    public interface IFileHelper
    {
        /// <summary>
        /// Checks if a file with the given filename exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// A task to await the exists operation, returning <code>true</code>
        /// if the file exists, otherwise <code>false</code>.
        /// </returns>
        Task<bool> ExistsAsync(string filename);

        /// <summary>
        /// Gets the last modified time from the file with the given name.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// A task to await the last modified operation, returning the last modified date.
        /// </returns>
        Task<DateTime> LastModifiedAsync(string filename);

        /// <summary>
        /// Writes text to the given filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="text">The text to write.</param>
        /// <returns>A task to await the write operation.</returns>
        Task WriteTextAsync(string filename, string text);

        /// <summary>
        /// Reads text from the given filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// A task to await the read operation, returning the full text of the file.
        /// </returns>
        Task<string> ReadTextAsync(string filename);

        /// <summary>
        /// Enumerates all files in the filesystem.
        /// </summary>
        /// <returns>
        /// A task to await the enumerate operation, returning the collection of files.
        /// </returns>
        Task<IEnumerable<string>> GetFilesAsync();

        /// <summary>
        /// Deletes a file with the given filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>A task to await the delete operation.</returns>
        Task DeleteAsync(string filename);
    }
}
