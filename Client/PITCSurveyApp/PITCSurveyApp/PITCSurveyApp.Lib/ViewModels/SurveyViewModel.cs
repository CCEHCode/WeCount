using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xamarin.Forms;

using PITCSurveyApp.Lib.Helpers;
using PITCSurveyApp.Lib.Services;
using PITCSurveyLib.Models;

namespace PITCSurveyApp.Lib.ViewModels
{
    /// <summary>
    /// ViewModel class used to manage
    /// </summary>
    public class SurveyViewModel
    {
        //public NotifyTaskCompletion<SurveyModel> Survey { get; private set; }
        private SurveyModel Survey { get; set;  }

        /// <summary>
        /// SurveyViewModel Constructor
        /// </summary>
        public SurveyViewModel()
        {
           
        }

        /// <summary>
        /// Load the surveys from local storage or the cloud
        /// </summary>
        /// <returns></returns>
        public async Task GetSurvey()
        {
            Exception error = null;
            try
            {
                // TO DO: Load the survey from JSON files in local storage, if not present
                // then load them from Azure
                Survey = await SurveyCloudService.GetLatestSurvey();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                error = ex;
            }
        }

        public string SurveyVersionCloud
        {
            // TO DO: Get the actual version from the survey in Azure
            get
            {
                return "1.0";
            }
        }

        public string SurveyVersionLocal
        {
            // TO DO: Get the actual version from the survey store locally
            get { return "1.0"; }
        }

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
