using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Owin;
using System.Configuration;
using System.Web.Http;

namespace PITCSurveySvc
{
	public partial class Startup
	{

		public static void ConfigureMobileApp(IAppBuilder app)
		{
			var config = new HttpConfiguration();

			new MobileAppConfiguration()
				.ApplyTo(config);

			config.MapHttpAttributeRoutes();

			var settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();
			if (string.IsNullOrEmpty(settings.HostName))
			{
				app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
				{
					SigningKey = ConfigurationManager.AppSettings["SigningKey"],
					ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
					ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
					TokenHandler = config.GetAppServiceTokenHandler()
				});
			}

			app.UseWebApi(config);
		}
	}
}