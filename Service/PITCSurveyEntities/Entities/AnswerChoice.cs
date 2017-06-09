using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PITCSurveyLib;

namespace PITCSurveyEntities.Entities
{
    public class AnswerChoice
    {
        [Key]
        public int ID { get; set; }

        public String Key { get; set; }

        public String AnswerText { get; set; }

        public AnswerFormat AdditionalAnswerDataFormat { get; set; }
    }
}
