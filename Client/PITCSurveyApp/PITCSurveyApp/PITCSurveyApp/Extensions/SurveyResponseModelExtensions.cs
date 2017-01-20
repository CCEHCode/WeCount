using System;
using System.Linq;
using System.Threading.Tasks;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyApp.Services;
using PITCSurveyLib;
using PITCSurveyLib.Models;
using Xamarin.Forms;

namespace PITCSurveyApp.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="SurveyResponseModel"/>.
    /// </summary>
    static class SurveyResponseModelExtensions
    {
        /// <summary>
        /// Searches the survey response for a well-known answer, e.g., initials or date of birth.
        /// </summary>
        /// <param name="response">The survey response.</param>
        /// <param name="survey">The survey.</param>
        /// <param name="question">The well-known question type.</param>
        /// <returns>
        /// The specified valid of the well-known answer, or <code>null</code> if not found.
        /// </returns>
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

        /// <summary>
        /// Uploads the survey asynchronously.
        /// </summary>
        /// <param name="response">The survey response.</param>
        /// <returns>A task to await the completion of the upload.</returns>
        public static async Task UploadAsync(this UploadedItem<SurveyResponseModel> response)
        {
            try
            {
                DependencyService.Get<IMetricsManagerService>().TrackEvent("UploadSurveyResponse");
                await SurveyCloudService.SubmitSurveyResponseAsync(response.Item);
                response.Uploaded = DateTime.Now;
                await response.SaveAsync();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMetricsManagerService>().TrackException("UploadSurveyResponseFailed", ex);
                throw;
            }
        }

        /// <summary>
        /// Saves the survey to local storage asynchronously.
        /// </summary>
        /// <param name="response">The survey response.</param>
        /// <returns>A task to await the save operation.</returns>
        public static Task SaveAsync(this UploadedItem<SurveyResponseModel> response)
        {
            var fileHelper = new FileHelper();
            return fileHelper.SaveAsync(response.Item.GetFilename(), response);
        }

        /// <summary>
        /// Deletes the survey from local storage asynchronously.
        /// </summary>
        /// <param name="response">The survey response.</param>
        /// <returns>A task to await the delete operation.</returns>
        public static Task DeleteAsync(this UploadedItem<SurveyResponseModel> response)
        {
            var fileHelper = new FileHelper();
            return fileHelper.DeleteAsync(response.Item.GetFilename());
        }

        /// <summary>
        /// Initializes a new survey response.
        /// </summary>
        /// <returns>The survey response.</returns>
        public static SurveyResponseModel CreateNew()
        {
            return new SurveyResponseModel
            {
                ResponseIdentifier = Guid.NewGuid(),
                SurveyID = App.LatestSurvey.SurveyID,
                Survey_Version = App.LatestSurvey.Version,
                StartTime = DateTimeOffset.Now,
            };
        }

        /// <summary>
        /// Gets the filename of the survey response, based on the survey response <see cref="Guid"/>.
        /// </summary>
        /// <param name="response">The survey response.</param>
        /// <returns>The filename.</returns>
        private static string GetFilename(this SurveyResponseModel response)
        {
            return $"{response.ResponseIdentifier}.survey.json";
        }
    }
}
