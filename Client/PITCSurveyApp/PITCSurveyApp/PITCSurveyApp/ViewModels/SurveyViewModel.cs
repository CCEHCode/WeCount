using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITCSurveyApp.Helpers;
using PITCSurveyApp.Models;
using PITCSurveyApp.Services;
using PITCSurveyLib.Models;

namespace PITCSurveyApp.ViewModels
{
    /// <summary>
    /// ViewModel class used to manage
    /// </summary>
    public class SurveyViewModel
    {
        private const string SurveyFileName = "questions.json";

        //public NotifyTaskCompletion<SurveyModel> Survey { get; private set; }
        private SurveyModel Survey { get; set;  }

        /// <summary>
        /// Load the surveys from local storage or the cloud
        /// </summary>
        /// <returns></returns>
        public async Task GetSurvey()
        {
            var fileHelper = new FileHelper();
            if (await fileHelper.ExistsAsync(SurveyFileName))
            {
                var surveyText = await fileHelper.ReadTextAsync(SurveyFileName);
                var surveyJson = JObject.Parse(surveyText);
                Survey = surveyJson.ToObject<SurveyModel>();
            }

            try
            {
                // TODO: add logic to only periodically check for survey updates
                var azureSurvey = await SurveyCloudService.GetLatestSurvey();
                if (Survey == null || Survey.Version < azureSurvey.Version)
                {
                    Survey = azureSurvey;
                    var surveyJson = JObject.FromObject(Survey);
                    var surveyText = surveyJson.ToString(Formatting.None);
                    await fileHelper.WriteTextAsync(SurveyFileName, surveyText);
                }
            }
            catch (Exception ex)
            {
                // TODO: capture exception in HockeyApp
            }
        }

        public string SurveyVersion => Survey?.Version.ToString();

        public int SurveyQuestionsCount
        {
            get { return ((Survey != null) ? Survey.Questions.Count : 0); }
        }

        public SurveyQuestionModel Question(int index)
        {
            if (index >= 0)
            {
                return Survey.Questions[index];
            }
            else
            {
                return null;
            }
        }
    }
}
