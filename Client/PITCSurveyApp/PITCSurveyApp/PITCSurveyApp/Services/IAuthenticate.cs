using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace PITCSurveyApp.Services
{
    public interface IAuthenticate
    {
        MobileServiceUser User { get; set; }

        Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters);

        Task<MobileServiceUser> RefreshLoginAsync();

        Task LogoutAsync();
    }
}
