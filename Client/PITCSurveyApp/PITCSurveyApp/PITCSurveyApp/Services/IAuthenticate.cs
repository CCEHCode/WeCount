using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace PITCSurveyApp.Services
{
    /// <summary>
    /// An interface for required authentication operations.
    /// </summary>
    public interface IAuthenticate
    {
        /// <summary>
        /// The currently authenticated user.
        /// </summary>
        /// <remarks>
        /// Use the setter with the cached user information (user ID and
        /// authentication token) before calling <see cref="RefreshLoginAsync"/>. 
        /// </remarks>
        MobileServiceUser User { get; set; }

        /// <summary>
        /// Login to the given authentication provider with the given parameters.
        /// </summary>
        /// <param name="provider">The authentication provider.</param>
        /// <param name="parameters">
        /// The login parameters, e.g., <code>{'access_type': 'offline'}</code>.
        /// </param>
        /// <returns>
        /// A task to await the login operation, returning the authenticated user.
        /// </returns>
        Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters);

        /// <summary>
        /// Refreshes the authentication token of the previously authenticated user.
        /// </summary>
        /// <returns>
        /// A task to await the refresh operation, returning the authenticated user.
        /// </returns>
        Task<MobileServiceUser> RefreshLoginAsync();

        /// <summary>
        /// Logout the currently authenticated user.
        /// </summary>
        /// <returns>A task to await the logout operation.</returns>
        Task LogoutAsync();
    }
}
