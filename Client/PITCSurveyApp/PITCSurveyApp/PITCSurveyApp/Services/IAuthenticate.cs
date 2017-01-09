using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace PITCSurveyApp.Services
{
    public interface IAuthenticate
    {
        Task<MobileServiceUser> AuthenticateAsync(MobileServiceAuthenticationProvider provider);
    }
}
