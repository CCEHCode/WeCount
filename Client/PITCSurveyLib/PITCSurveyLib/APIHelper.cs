using Microsoft.Rest;
using PITCSurveyLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITCSurveyLib
{
	public static class APIHelper
	{

		private const string AzureMobileAppUrl = "https://pitcsurveyapi.azurewebsites.net";

		private static readonly ServiceClientCredentials _AnonCreds = new Microsoft.Rest.BasicAuthenticationCredentials() { UserName = "", Password = "" };
		
		private static PITCSurveyAPI _API;

		// TODO: Could expose an actual ServiceClientCredentials property here, but I don't think it'll be that useful for us.
		private static string _AuthToken;
		public static string AuthToken
		{
			get
			{
				return _AuthToken;
			}

			set
			{
				_AuthToken = value;

				InitAPI();
			}
		}

		static APIHelper()
		{
			InitAPI();
		}

		private static void InitAPI()
		{
			_API?.Dispose();

			_API = new PITCSurveyAPI(new Uri(AzureMobileAppUrl), string.IsNullOrEmpty(AuthToken) ? _AnonCreds : new Microsoft.Rest.TokenCredentials(AuthToken));
		}

		public static void SignOut()
		{
			AuthToken = null;
		}

		public static IEnumerable<SurveySummaryModel> GetAvailableSurveys()
		{
			return GetAvailableSurveysAsync().Result;
		}

		public static async Task<IEnumerable<SurveySummaryModel>> GetAvailableSurveysAsync()
		{
			try
			{
				var result = await _API.GetAllSurveysAsync(true);

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

		public static SurveyModel GetSurveyByID(int ID)
		{
			return GetSurveyByIDAsync(ID).Result;
		}

		public static async Task<SurveyModel> GetSurveyByIDAsync(int ID)
		{
			try
			{
				var result = await _API.GetSurveyByIDAsync(ID);

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

		public static bool SubmitSurveyResponse(SurveyResponseModel SurveyResponse)
		{
			return SubmitSurveyResponseAsync(SurveyResponse).Result;
		}

		public static async Task<bool> SubmitSurveyResponseAsync(SurveyResponseModel SurveyResponse)
		{
			try
			{
				await _API.PostSurveyResponseAsync(SurveyResponse);

				return true;
			}
			catch (HttpOperationException hex)
			{
				switch (hex.Response.StatusCode)
				{
					case System.Net.HttpStatusCode.NoContent:
						// NOTE: This does *not* throw error, but returns true above.
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
