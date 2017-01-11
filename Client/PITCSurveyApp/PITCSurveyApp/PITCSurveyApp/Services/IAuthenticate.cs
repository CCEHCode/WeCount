using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace PITCSurveyApp.Services
{
    public interface IAuthenticate
    {
        MobileServiceUser User { get; set; }

        Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider);

        Task RefreshLoginAsync();

        Task LogoutAsync();
    }
}
