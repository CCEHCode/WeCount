using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PITCSurveyApp.Services;

namespace PITCSurveyApp.UWP
{
    public sealed partial class MainPage : IAuthenticate
    {
        public MainPage()
        {
            this.InitializeComponent();
            PITCSurveyApp.App.Init(this);
            LoadApplication(new PITCSurveyApp.App());
        }

        public async Task<MobileServiceUser> AuthenticateAsync(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                return await SurveyCloudService.ApiClient.LoginAsync(provider);
            }
            catch
            {
                return null;
            }
        }
    }
}
