using System;

namespace PITCSurveyApp.Models
{
    public class UploadedItem<T>
    {
        public T Item { get; set; }

        public DateTime? Uploaded { get; set; }
    }
}
