using LoginPageAPI.DTOs;
using LoginPageAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginPageAPI.Services
{
    /// <summary>
    /// User management service implementation.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<object>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var list = new List<object>();
            foreach (var user in users)
            {
                list.Add(new {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                });
            }
            return list;
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? new List<string>() : await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) != null;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}
