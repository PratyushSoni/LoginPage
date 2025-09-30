using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoginPageAPI.Services
{
    public interface IRoleService
    {
        Task<IList<string>> GetAllRolesAsync();
        Task<bool> RoleExistsAsync(string roleName);
        Task<bool> CreateRoleAsync(string roleName);
        // Add more role-related methods as needed
    }
}
