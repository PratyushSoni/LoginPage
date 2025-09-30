using LoginPageAPI.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LoginPageAPI.Services
{
    /// <summary>
    /// JWT token generation operations.
    /// </summary>
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles);
        IEnumerable<Claim> GetClaims(ApplicationUser user, IList<string> roles);
    }
}
