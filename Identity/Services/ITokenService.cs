using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user, IEnumerable<string> roles);

        /// <summary>
        /// Gets the token expiry time in seconds
        /// </summary>
        /// <returns>Token expiry in seconds</returns>
        int GetTokenExpiryInSeconds();
    }
}
