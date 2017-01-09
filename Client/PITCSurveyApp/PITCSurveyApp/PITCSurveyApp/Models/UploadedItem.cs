using System;

namespace PITCSurveyApp.Models
{
    public class UploadedItem<T>
    {
        public UploadedItem(T item)
        {
            Item = item;
        }

        public T Item { get; set; }

        public DateTime? Uploaded { get; set; }
    }
}
