using PITCSurveyEntities.Entities;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Diagnostics;

namespace PITCSurveySvc.Controllers
{
	public abstract class BaseController : ApiController
    {
		const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";

		protected PITCSurveyContext db = new PITCSurveyContext();

		protected Volunteer GetAuthenticatedVolunteer()
		{
			Trace.TraceInformation($"User authenticated: {User.Identity.IsAuthenticated}");

			if (User.Identity.IsAuthenticated)
			{
				try
				{
					Trace.TraceInformation($"User is authenticated. Name: {User.Identity?.Name}, {User.Identity?.AuthenticationType}");

					var ClaimsPrincipal = this.User as ClaimsPrincipal;

					Trace.TraceInformation($"NameIdentifier: {((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value}");

					string sid = ClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;

					Trace.TraceInformation($"SID: {sid}");

					var Volunteer = db.Volunteers.Where(v => v.AuthID == sid).SingleOrDefault();

					if (Volunteer == null)
					{
						Trace.TraceInformation("Adding new volunteer.");

						// Volunteer not already recognized, add.

						Volunteer = new Volunteer { AuthID = sid, AuthProvider = ClaimsPrincipal.FindFirst(IdentityProvider)?.Value };

						db.Volunteers.Add(Volunteer);
					}

					// If existing volunteer, update profile info

					Volunteer.Email = ClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
					Volunteer.FirstName = ClaimsPrincipal.FindFirst(ClaimTypes.GivenName)?.Value;
					Volunteer.LastName = ClaimsPrincipal.FindFirst(ClaimTypes.Surname)?.Value;
					Volunteer.HomePhone = ClaimsPrincipal.FindFirst(ClaimTypes.HomePhone)?.Value;
					Volunteer.MobilePhone = ClaimsPrincipal.FindFirst(ClaimTypes.MobilePhone)?.Value;

					Volunteer.Address.Street = ClaimsPrincipal.FindFirst(ClaimTypes.StreetAddress)?.Value;
					Volunteer.Address.City = ClaimsPrincipal.FindFirst(ClaimTypes.Locality)?.Value;
					Volunteer.Address.State = ClaimsPrincipal.FindFirst(ClaimTypes.StateOrProvince)?.Value;
					Volunteer.Address.ZipCode = ClaimsPrincipal.FindFirst(ClaimTypes.PostalCode)?.Value;

					// TODO: Use new scoped context, so as not to inadvertently save any other changes? UOW
					// If so, return from primary context to allow attachment
					db.SaveChanges();

					Trace.TraceInformation($"Volunteer ID: {Volunteer.ID}");

					return Volunteer;
				}
				catch (System.Exception ex)
				{
					Trace.TraceError("Error playing with creds: " + ex.ToString());
				}
			}

			return null;
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
