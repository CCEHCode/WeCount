using System;

namespace PITCSurveyApp.Models
{
    /// <summary>
    /// A wrapper class for tracking the time an item was uploaded.
    /// </summary>
    /// <typeparam name="T">Type of item uploaded.</typeparam>
    public class UploadedItem<T>
    {
        /// <summary>
        /// Instantiates the uploaded item.
        /// </summary>
        /// <param name="item">The item to wrap.</param>
        public UploadedItem(T item)
        {
            Item = item;
        }

        /// <summary>
        /// The wrapped item.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// The time uploaded, or <code>null</code> if the item hasn't been uploaded.
        /// </summary>
        public DateTime? Uploaded { get; set; }
    }
}
