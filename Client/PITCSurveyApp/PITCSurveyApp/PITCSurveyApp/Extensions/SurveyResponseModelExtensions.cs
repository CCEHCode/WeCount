using System.Linq;
using PITCSurveyLib;
using PITCSurveyLib.Models;

namespace PITCSurveyApp.Extensions
{
    static class SurveyResponseModelExtensions
    {
        public static string GetWellKnownAnswer(this SurveyResponseModel response, SurveyModel survey, WellKnownQuestion question)
        {
            var nameQuestion = survey.Questions
                .FirstOrDefault(q => q.WellKnownQuestion == question);

            if (nameQuestion == null)
            {
                return null;
            }

            return response.QuestionResponses
                .FirstOrDefault(r => r.QuestionID == nameQuestion.QuestionID)?
                .AnswerChoiceResponses?.FirstOrDefault()?
                .AdditionalAnswerData;
        }
    }
}
