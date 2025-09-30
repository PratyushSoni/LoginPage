using System.Threading.Tasks;
using System.Collections.Generic;

namespace LoginPageAPI.Services
{
    public interface IUserService
    {
        Task<bool> CheckUsernameExistsAsync(string username);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IList<object>> GetAllUsersAsync();
    }
}
