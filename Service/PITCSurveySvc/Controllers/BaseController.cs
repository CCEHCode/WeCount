using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Authentication;
using PITCSurveyEntities.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

namespace PITCSurveySvc.Controllers
{
	public abstract class BaseController : ApiController
    {
		const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";

		protected PITCSurveyContext db = new PITCSurveyContext();

		protected TelemetryClient Telemetry = new TelemetryClient();

		protected Volunteer GetAuthenticatedVolunteer(Guid? DeviceID = null)
		{
			Trace.TraceInformation($"User authenticated: {User.Identity.IsAuthenticated}, DeviceID: {DeviceID?.ToString()}");

			Volunteer Volunteer = null;

			if (User.Identity.IsAuthenticated)
			{
				try
				{
					Trace.TraceInformation($"User is authenticated. Name: {User.Identity?.Name}, {User.Identity?.AuthenticationType}");

					ClaimsIdentity Claimant = (ClaimsIdentity)User.Identity;

					string sid = Claimant.FindFirst(ClaimTypes.NameIdentifier)?.Value;

					Trace.TraceInformation($"SID: {sid}");

					Volunteer = db.Volunteers.Where(v => v.AuthID == sid).SingleOrDefault();

					if (Volunteer == null)
					{
						// TODO: If we have a DeviceID match, should we link this to that volunteer? Probably...

						Trace.TraceInformation("Adding new volunteer.");

						// Volunteer not already recognized, add.

						Volunteer = new Volunteer()	{ AuthID = sid, AuthProvider = Claimant.FindFirst(IdentityProvider)?.Value };

						FillProviderDetails(Claimant, Volunteer);

						db.Volunteers.Add(Volunteer);

						db.SaveChanges();

						Telemetry.TrackEvent("NewVolunteer", new Dictionary<string, string>() { { "IdType", Volunteer.AuthProvider }, { "VolunteerId", Volunteer.ID.ToString() } });
					}

					// TODO: Remove this later, for now useful for debugging
					FillProviderDetails(Claimant, Volunteer);
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error playing with creds (Authenticated): " + ex.ToString());
					Telemetry.TrackException(ex);
					throw;
				}
			}
			else if (DeviceID.HasValue && DeviceID.Value != Guid.Empty)
			{
				try
				{
					// Not authenticated, but does supply DeviceID, so also create record. Used when PUTing Volunteer info when not authenticated.

					Trace.TraceInformation($"User not authenticated, but supplied DeviceID: {DeviceID.ToString()}");

					Volunteer = db.Volunteers.Where(v => v.DeviceId == DeviceID.Value).OrderByDescending(v => v.ID).FirstOrDefault();

					if (Volunteer == null)
					{
						Trace.TraceInformation("Adding new volunteer.");

						// Volunteer not already recognized, add.

						Volunteer = new Volunteer { DeviceId = DeviceID.Value };

						db.Volunteers.Add(Volunteer);

						db.SaveChanges();

						Telemetry.TrackEvent("NewVolunteer", new Dictionary<string, string>() { { "IdType", "DeviceId" }, { "VolunteerId", Volunteer.ID.ToString() } });
					}
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error playing with creds (DeviceID): " + ex.ToString());
					Telemetry.TrackException(ex);
					throw;
				}
			}

			if (Volunteer != null)
			{
				try
				{
					Trace.TraceInformation($"Volunteer ID: {Volunteer.ID}");

					// Link DeviceID

					if (Volunteer.DeviceId == null && DeviceID.HasValue && DeviceID.Value != Guid.Empty)
					{
						// We have not previously associated this volunteer with a DeviceID, and one is available.

						Trace.TraceInformation("Linking volunteer to the DeviceID.");

						Telemetry.TrackEvent("NewVolunteer", new Dictionary<string, string>() { { "IdType", "Auth" } });

						Volunteer.DeviceId = DeviceID;

						Telemetry.TrackEvent("VolunteerIdentified", new Dictionary<string, string>() { { "DeviceId", Volunteer.DeviceId.ToString() }, { "VolunteerId", Volunteer.ID.ToString() } });

						// Link previously uploaded Responses from this DeviceID to this Volunteer (only for ones not already linked to a Volunteer)

						var PrevResponses = db.SurveyResponses.Where(r => r.DeviceId == DeviceID.Value && r.Volunteer == null).ToList();

						if (PrevResponses.Count() > 0)
						{
							Trace.TraceInformation($"Will link {PrevResponses.Count()} previously uploaded SurveyResponses to this volunteer based on DeviceID.");

							Telemetry.TrackEvent("ResponsesLinked", new Dictionary<string, string>() { { "DeviceId", Volunteer.DeviceId.ToString() }, { "VolunteerId", Volunteer.ID.ToString() }, { "ResponseCount", PrevResponses.Count().ToString() } });

							PrevResponses.ForEach(r => r.Volunteer = Volunteer);
						}

						db.SaveChanges();
					}
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error checking for link updates: " + ex.ToString());
					Telemetry.TrackException(ex);
					throw;
				}
			}

			return Volunteer;
		}

		/// <summary>
		/// Attempts to pull user details from provider to set initial defaults.
		/// </summary>
		/// <param name="Claimant"></param>
		/// <param name="Volunteer"></param>
		private void FillProviderDetails(ClaimsIdentity Claimant, Volunteer Volunteer)
		{
			Trace.TraceInformation($"User email (via provider): {Claimant?.FindFirst(ClaimTypes.Email)?.Value}");
			
			// Show claims in HttpContext.User.Identity.Claims
			
			//foreach (var Claim in (ClaimsPrincipal)HttpContext.Current.User.Identity)
			{

			}

			// Show claims in ClaimsIdentity
			if (Claimant != null && Claimant.Claims.Any())
			{
				Trace.TraceInformation("ClaimsIdentity claims:");
				Trace.Indent();

				foreach (var Claim in Claimant.Claims)
				{
					Trace.TraceInformation($"Type: {Claim.Type}, Value: {Claim.Value}, Subject: {Claim.Subject}");
				}

				Trace.Unindent();

				Volunteer.FirstName = Claimant?.FindFirst(ClaimTypes.GivenName)?.Value;
				Volunteer.LastName = Claimant?.FindFirst(ClaimTypes.Surname)?.Value;
				Volunteer.Email = Claimant?.FindFirst(ClaimTypes.Email)?.Value;
			}

			ProviderCredentials Creds = null;

			// Try to get details from provider
			switch (Claimant.FindFirst(IdentityProvider)?.Value)
			{
				case "google":
					Trace.TraceInformation("Trying to get Google profile info...");
					Creds = AwaitTask(this.User.GetAppServiceIdentityAsync<GoogleCredentials>(this.Request), 3000);
					break;

				case "microsoftaccount":
					Trace.TraceInformation("Trying to get Microsoft profile info...");
					Creds = AwaitTask(this.User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(this.Request), 3000);
					break;
			}

			Trace.Indent();

			Trace.TraceInformation($"Creds null: {(Creds == null)}, id: {Creds?.UserId}, count: {Creds?.UserClaims.Count().ToString()}");

			// Show claims in ProviderCredentials
			if (Creds != null && Creds.UserClaims.Any())
			{
				Trace.TraceInformation("ProviderCredentials claims:");
				Trace.Indent();

				foreach (var Claim in Creds.UserClaims)
				{
					Trace.TraceInformation($"Type: {Claim.Type}, Value: {Claim.Value}, Subject: {Claim.Subject}");
				}

				Trace.Unindent();
			}

			Trace.Unindent();
		}

		/// <summary>
		/// Helper to return sync value from async task.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Task"></param>
		/// <param name="Timeout"></param>
		/// <returns></returns>
		static T AwaitTask<T>(Task<T> Task, int Timeout) where T : class
		{
			Task.Wait(Timeout);

			return (Task.IsCompleted) ? Task.Result : null;
		}

		#region "IDisposable"

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

	}
}
