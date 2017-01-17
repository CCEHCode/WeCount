using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Google;

namespace PITCSurveySvc
{
	public partial class Startup
	{

		public static void ConfigureAuth(IAppBuilder app)
		{
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("")
			});

			var GAuth = new GoogleOAuth2AuthenticationOptions();

			GAuth.Scope.Add("email");
			GAuth.Provider = new GoogleOAuth2AuthenticationProvider()
			{
				OnAuthenticated = async context =>
				{
					context.Identity.AddClaim(new System.Security.Claims.Claim("GoogleAccessToken", context.AccessToken));
				}
			};

			//GAuth.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
		}
	}
}