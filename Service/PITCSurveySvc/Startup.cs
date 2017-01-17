using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PITCSurveySvc.Startup))]

namespace PITCSurveySvc
{
	public partial class Startup
	{

		public void Configuration(IAppBuilder app)
		{

			//ConfigureAuth(app);

			ConfigureMobileApp(app);
		}
	}
}