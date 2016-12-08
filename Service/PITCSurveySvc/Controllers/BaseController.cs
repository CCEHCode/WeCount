using PITCSurveyEntities.Entities;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;

namespace PITCSurveySvc.Controllers
{
	public abstract class BaseController : ApiController
    {
		const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";

		protected PITCSurveyContext db = new PITCSurveyContext();

		protected Volunteer GetAuthenticatedVolunteer()
		{
			if (User.Identity.IsAuthenticated)
			{
				var ClaimsPrincipal = this.User as ClaimsPrincipal;

				string sid = ClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;

				var Volunteer = db.Volunteers.Where(v => v.AuthID == sid).SingleOrDefault();

				if (Volunteer == null)
				{
					// Volunteer not already recognized, add.

					Volunteer = new Volunteer
					{
						AuthID = sid,
						AuthProvider = ClaimsPrincipal.FindFirst(IdentityProvider).Value,
						Email = ClaimsPrincipal.FindFirst(ClaimTypes.Email).Value,
						FirstName = ClaimsPrincipal.FindFirst(ClaimTypes.GivenName).Value,
						LastName = ClaimsPrincipal.FindFirst(ClaimTypes.Surname).Value,
						HomePhone = ClaimsPrincipal.FindFirst(ClaimTypes.HomePhone).Value,
						MobilePhone = ClaimsPrincipal.FindFirst(ClaimTypes.MobilePhone).Value
					};

					Volunteer.Address.Street = ClaimsPrincipal.FindFirst(ClaimTypes.StreetAddress).Value;
					Volunteer.Address.City = ClaimsPrincipal.FindFirst(ClaimTypes.Locality).Value;
					Volunteer.Address.State = ClaimsPrincipal.FindFirst(ClaimTypes.StateOrProvince).Value;
					Volunteer.Address.ZipCode = ClaimsPrincipal.FindFirst(ClaimTypes.PostalCode).Value;

					db.Volunteers.Add(Volunteer);

					// TODO: Use new scoped context, so as not to inadvertently save any other changes? UOW
					db.SaveChanges();
				}

				return Volunteer;
			}
			else
			{
				return null;
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
