using PITCSurveyEntities.Entities;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Diagnostics;
using System;

namespace PITCSurveySvc.Controllers
{
	public abstract class BaseController : ApiController
    {
		const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";

		protected PITCSurveyContext db = new PITCSurveyContext();

		protected Volunteer GetAuthenticatedVolunteer(Guid? DeviceID = null)
		{
			Trace.TraceInformation($"User authenticated: {User.Identity.IsAuthenticated}");

			Volunteer Volunteer = null;

			if (User.Identity.IsAuthenticated)
			{
				try
				{
					Trace.TraceInformation($"User is authenticated. Name: {User.Identity?.Name}, {User.Identity?.AuthenticationType}");

					var Claimant = (ClaimsIdentity)User.Identity;

					string sid = Claimant.FindFirst(ClaimTypes.NameIdentifier)?.Value;

					Trace.TraceInformation($"SID: {sid}");

					Volunteer = db.Volunteers.Where(v => v.AuthID == sid).SingleOrDefault();

					if (Volunteer == null)
					{
						Trace.TraceInformation("Adding new volunteer.");

						// Volunteer not already recognized, add.

						Volunteer = new Volunteer { AuthID = sid, AuthProvider = Claimant.FindFirst(IdentityProvider)?.Value };

						db.Volunteers.Add(Volunteer);

						db.SaveChanges();
					}
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error playing with creds: " + ex.ToString());
				}
			}
			else if (DeviceID.HasValue && DeviceID.Value != Guid.Empty)
			{
				// Not authenticated, but does supply DeviceID, so also create record. Used when PUTing Volunteer info when not authenticated.

				Volunteer = db.Volunteers.Where(v => v.DeviceId == DeviceID.Value).SingleOrDefault();

				if (Volunteer == null)
				{
					Trace.TraceInformation("Adding new volunteer.");

					// Volunteer not already recognized, add.

					Volunteer = new Volunteer { DeviceId = DeviceID.Value };

					db.Volunteers.Add(Volunteer);

					db.SaveChanges();
				}
			}

			if (Volunteer != null)
			{
				Trace.TraceInformation($"Volunteer ID: {Volunteer.ID}");
				
				// Link DeviceID

				if (Volunteer.DeviceId == null && DeviceID.HasValue && DeviceID.Value != Guid.Empty)
				{
					// We have not previously associated this volunteer with a DeviceID, and one is available.

					Volunteer.DeviceId = DeviceID;

					// Link previously uploaded Responses from this DeviceID to this Volunteer (only for ones not already linked to a Volunteer)

					db.SurveyResponses.Where(r => r.DeviceId == DeviceID.Value && r.Volunteer == null).ToList().ForEach(r => r.Volunteer = Volunteer);

					db.SaveChanges();
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
