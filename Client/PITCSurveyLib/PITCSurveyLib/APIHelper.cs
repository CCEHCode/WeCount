using Microsoft.Rest;
using PITCSurveyLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITCSurveyLib
{
	public class APIHelper
	{

		private const string AzureMobileAppUrl = "https://pitcsurveyapi.azurewebsites.net";

		private PITCSurveyAPI API;

		public APIHelper()
		{
			var Creds = new Microsoft.Rest.BasicAuthenticationCredentials() { UserName = "", Password = "" };

			API = new PITCSurveyAPI(new Uri(AzureMobileAppUrl), Creds);
		}

		public IEnumerable<SurveySummaryModel> GetAvailableSurveys()
		{
			return GetAvailableSurveysAsync().Result;
		}

		public async Task<IEnumerable<SurveySummaryModel>> GetAvailableSurveysAsync()
		{
			try
			{
				var result = await API.GetAllSurveysAsync(true);

				return result;
			}
			catch (HttpOperationException hex)
			{
				throw new APIException(hex.Message, hex.Response.StatusCode);
			}
			catch (Exception ex)
			{
				throw new APIException(ex.Message, ex);
			}
		}

		public SurveyModel GetSurveyByID(int ID)
		{
			return GetSurveyByIDAsync(ID).Result;
		}

		public async Task<SurveyModel> GetSurveyByIDAsync(int ID)
		{
			try
			{
				var result = await API.GetSurveyByIDAsync(ID);

				return result;
			}
			catch (HttpOperationException hex)
			{
				throw new APIException(hex.Message, hex.Response.StatusCode);
			}
			catch (Exception ex)
			{
				throw new APIException(ex.Message, ex);
			}
		}

		public bool SubmitSurveyResponse(SurveyResponseModel SurveyResponse)
		{
			return SubmitSurveyResponseAsync(SurveyResponse).Result;
		}

		public async Task<bool> SubmitSurveyResponseAsync(SurveyResponseModel SurveyResponse)
		{
			try
			{
				await API.PostSurveyResponseAsync(SurveyResponse);

				return true;
			}
			catch (HttpOperationException hex)
			{
				switch (hex.Response.StatusCode)
				{
					case System.Net.HttpStatusCode.NoContent:
						return true;
					case System.Net.HttpStatusCode.Conflict:
						return false;
					default:
						throw new APIException($"{hex.Message} ({hex.Response.ReasonPhrase}): {hex.Response.Content}", hex.Response.StatusCode, hex);
				}
			}
			catch (Exception ex)
			{
				throw new APIException(ex.Message, ex);
			}
		}

	}
}
