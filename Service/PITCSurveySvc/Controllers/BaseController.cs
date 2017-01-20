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
		const string identityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";

		protected PITCSurveyContext db = new PITCSurveyContext();

		protected TelemetryClient telemetry = new TelemetryClient();

		protected async Task<Volunteer> GetAuthenticatedVolunteerAsync(Guid? deviceID = null)
		{
			Trace.TraceInformation($"User authenticated: {User.Identity.IsAuthenticated}, DeviceID: {deviceID?.ToString()}");

			Volunteer volunteer = null;

			if (User.Identity.IsAuthenticated)
			{
				try
				{
					Trace.TraceInformation($"User is authenticated. Name: {User.Identity?.Name}, {User.Identity?.AuthenticationType}");

					ClaimsIdentity claimant = (ClaimsIdentity)User.Identity;

					string sid = claimant.FindFirst(ClaimTypes.NameIdentifier)?.Value;

					Trace.TraceInformation($"SID: {sid}");

					volunteer = db.Volunteers.Where(v => v.AuthID == sid).SingleOrDefault();

					if (volunteer == null)
					{
						// TODO: If we have a DeviceID match, should we link this to that volunteer? Probably...

						Trace.TraceInformation("Adding new volunteer.");

						// Volunteer not already recognized, add.

						volunteer = new Volunteer()	{ AuthID = sid, AuthProvider = claimant.FindFirst(identityProvider)?.Value };

						await FillProviderDetails(claimant, volunteer);

						db.Volunteers.Add(volunteer);

						db.SaveChanges();

						telemetry.TrackEvent("NewVolunteer", new Dictionary<string, string>() { { "IdType", volunteer.AuthProvider }, { "VolunteerId", volunteer.ID.ToString() } });
					}
					else
					{
						// For now, back-fill existing accounts as well.
						await FillProviderDetails(claimant, volunteer);
						db.SaveChanges();
					}
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error playing with creds (Authenticated): " + ex.ToString());
					telemetry.TrackException(ex);
					throw;
				}
			}
			else if (deviceID.HasValue && deviceID.Value != Guid.Empty)
			{
				try
				{
					// Not authenticated, but does supply DeviceID, so also create record. Used when PUTing Volunteer info when not authenticated.

					Trace.TraceInformation($"User not authenticated, but supplied DeviceID: {deviceID.ToString()}");

					volunteer = db.Volunteers.Where(v => v.DeviceId == deviceID.Value).OrderByDescending(v => v.ID).FirstOrDefault();

					if (volunteer == null)
					{
						Trace.TraceInformation("Adding new volunteer.");

						// Volunteer not already recognized, add.

						volunteer = new Volunteer { DeviceId = deviceID.Value };

						db.Volunteers.Add(volunteer);

						db.SaveChanges();

						telemetry.TrackEvent("NewVolunteer", new Dictionary<string, string>() { { "IdType", "DeviceId" }, { "VolunteerId", volunteer.ID.ToString() } });
					}
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error playing with creds (DeviceID): " + ex.ToString());
					telemetry.TrackException(ex);
					throw;
				}
			}

			if (volunteer != null)
			{
				try
				{
					Trace.TraceInformation($"Volunteer ID: {volunteer.ID}");

					// Link DeviceID

					if (volunteer.DeviceId == null && deviceID.HasValue && deviceID.Value != Guid.Empty)
					{
						// We have not previously associated this volunteer with a DeviceID, and one is available.

						Trace.TraceInformation("Linking volunteer to the DeviceID.");

						telemetry.TrackEvent("NewVolunteer", new Dictionary<string, string>() { { "IdType", "Auth" } });

						volunteer.DeviceId = deviceID;

						telemetry.TrackEvent("VolunteerIdentified", new Dictionary<string, string>() { { "DeviceId", volunteer.DeviceId.ToString() }, { "VolunteerId", volunteer.ID.ToString() } });

						// Link previously uploaded Responses from this DeviceID to this Volunteer (only for ones not already linked to a Volunteer)

						var PrevResponses = db.SurveyResponses.Where(r => r.DeviceId == deviceID.Value && r.Volunteer == null).ToList();

						if (PrevResponses.Count() > 0)
						{
							Trace.TraceInformation($"Will link {PrevResponses.Count()} previously uploaded SurveyResponses to this volunteer based on DeviceID.");

							telemetry.TrackEvent("ResponsesLinked", new Dictionary<string, string>() { { "DeviceId", volunteer.DeviceId.ToString() }, { "VolunteerId", volunteer.ID.ToString() }, { "ResponseCount", PrevResponses.Count().ToString() } });

							PrevResponses.ForEach(r => r.Volunteer = volunteer);
						}

						db.SaveChanges();
					}
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error checking for link updates: " + ex.ToString());
					telemetry.TrackException(ex);
					throw;
				}
			}

			return volunteer;
		}

		/// <summary>
		/// Attempts to pull user details from provider to set initial defaults.
		/// </summary>
		/// <param name="claimant"></param>
		/// <param name="volunteer"></param>
		private async Task FillProviderDetails(ClaimsIdentity claimant, Volunteer volunteer)
		{
			ProviderCredentials creds = null;

			// Try to get details from provider
			switch (claimant.FindFirst(identityProvider)?.Value)
			{
				case "google":
					Trace.TraceInformation("Trying to get Google profile info...");
					creds = await this.User.GetAppServiceIdentityAsync<GoogleCredentials>(this.Request);
					break;

				case "microsoftaccount":
					Trace.TraceInformation("Trying to get Microsoft profile info...");
					creds = await this.User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(this.Request);
					break;
			}

			// Use claims in ProviderCredentials
			if (creds != null && creds.UserClaims.Any())
			{
				volunteer.FirstName = volunteer.FirstName ?? creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
				volunteer.LastName = volunteer.LastName ?? creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
				volunteer.Email = creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
				volunteer.MobilePhone = creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value ?? volunteer.MobilePhone;
				volunteer.HomePhone = volunteer.HomePhone ?? creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.HomePhone)?.Value;

				volunteer.Address.Street = volunteer.Address.Street ?? creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress)?.Value;
				volunteer.Address.City = volunteer.Address.City ?? creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Locality)?.Value;
				volunteer.Address.State = volunteer.Address.State ?? creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.StateOrProvince)?.Value;
				volunteer.Address.ZipCode = volunteer.Address.ZipCode ?? creds.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.PostalCode)?.Value;

				Trace.TraceInformation($"Volunteer profile (Provider): {volunteer.Email}, {volunteer.LastName}, {volunteer.FirstName}, {volunteer.MobilePhone}, {volunteer.HomePhone}");
				Trace.TraceInformation($"    {volunteer.Address.Street}, {volunteer.Address.City}, {volunteer.Address.State} {volunteer.Address.ZipCode}");
			}
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
