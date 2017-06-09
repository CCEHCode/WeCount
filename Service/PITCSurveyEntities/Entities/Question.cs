using PITCSurveyLib;
using System;
using System.ComponentModel.DataAnnotations;

namespace PITCSurveyEntities.Entities
{
    public class Question
    {
        [Key]
        public int ID { get; set; }

        public String Key { get; set; }

        public String QuestionText { get; set; }
        public String ClarificationText { get; set; }

        public WellKnownQuestion WellKnownQuestion { get; set; }

        public bool AllowMultipleAnswers { get; set; }

        //public virtual IList<AnswerChoice> AnswerChoices { get; set; }
    }
}