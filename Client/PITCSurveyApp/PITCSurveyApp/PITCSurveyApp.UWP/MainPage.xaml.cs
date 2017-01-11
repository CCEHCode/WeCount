using System.Collections.Generic;
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

        public MobileServiceUser User
        {
            get { return SurveyCloudService.ApiClient.CurrentUser; }
            set { SurveyCloudService.ApiClient.CurrentUser = value; }
        }

        public Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters)
        {
            return SurveyCloudService.ApiClient.LoginAsync(provider, parameters);
        }

        public Task<MobileServiceUser> RefreshLoginAsync()
        {
            return SurveyCloudService.ApiClient.RefreshUserAsync();
        }

        public Task LogoutAsync()
        {
            return SurveyCloudService.ApiClient.LogoutAsync();
        }
    }
}
