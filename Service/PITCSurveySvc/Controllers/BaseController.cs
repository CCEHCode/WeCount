using PITCSurveyEntities.Entities;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Diagnostics;
using System;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace PITCSurveySvc.Controllers
{
	public abstract class BaseController : ApiController
    {
		const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";

		protected PITCSurveyContext db = new PITCSurveyContext();

		protected TelemetryClient Telemetry = new TelemetryClient();

		protected Volunteer GetAuthenticatedVolunteer(Guid? DeviceID = null)
		{
			Trace.TraceInformation($"User authenticated: {User.Identity.IsAuthenticated}, DeviceID: {DeviceID.ToString()}");

			Volunteer Volunteer = null;

			if (User.Identity.IsAuthenticated)
			{
				try
				{
					Trace.TraceInformation($"User is authenticated. Name: {User.Identity?.Name}, {User.Identity?.AuthenticationType}");

					ClaimsIdentity ci = (ClaimsIdentity)User.Identity;

					Trace.TraceInformation($"User email (via provider): {ci.FindFirst(ClaimTypes.Email)?.Value}");

					var Claimant = (ClaimsIdentity)User.Identity;

					string sid = Claimant.FindFirst(ClaimTypes.NameIdentifier)?.Value;

					Trace.TraceInformation($"SID: {sid}");

					Volunteer = db.Volunteers.Where(v => v.AuthID == sid).SingleOrDefault();

					if (Volunteer == null)
					{
						Trace.TraceInformation("Adding new volunteer.");

						// Volunteer not already recognized, add.

						Volunteer = new Volunteer
						{
							AuthID = sid,
							AuthProvider = Claimant.FindFirst(IdentityProvider)?.Value,
							FirstName = ci.FindFirst(ClaimTypes.GivenName)?.Value,
							LastName = ci.FindFirst(ClaimTypes.Surname)?.Value,
							Email = ci.FindFirst(ClaimTypes.Email)?.Value
						};

						db.Volunteers.Add(Volunteer);

						db.SaveChanges();

						Telemetry.TrackEvent("NewVolunteer", new Dictionary<string, string>() { { "IdType", Volunteer.AuthProvider }, { "VolunteerId", Volunteer.ID.ToString() } });
					}
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
