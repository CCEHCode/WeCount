using System;
using PITCSurveyLib.Models;

namespace PITCSurveyApp.Models
{
    class SurveyResponseModelWrapper
    {
        public SurveyResponseModel Response { get; set; }

        public string Name { get; set; }

        public string DateOfBirth { get; set; }

        public DateTime? Uploaded { get; set; }
    }
}
