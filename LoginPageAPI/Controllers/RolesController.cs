using LoginPageAPI.DTOs;
using LoginPageAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginPageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _cache;

        public RolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMemoryCache cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cache = cache;
        }

        // POST: api/roles/assign
        // 🔒 Only Admin can assign roles
        [HttpPost("assign")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.Role))
                return BadRequest("UserId and Role are required.");

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
                return BadRequest($"Role '{dto.Role}' does not exist.");

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(dto.Role))
                return BadRequest($"User already has role '{dto.Role}'.");

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (result.Succeeded)
            {
                // Remove user cache if exists
                _cache.Remove($"user:{user.UserName}");
                // Remove roles cache
                _cache.Remove("roles:list");
                return Ok($"Role '{dto.Role}' assigned to user '{user.UserName}'.");
            }
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        // GET: api/roles
        // 🔒 Admin-only
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult GetRoles()
        {
            var cacheKey = "roles:list";
            if (!_cache.TryGetValue<List<string>>(cacheKey, out var roles))
            {
                roles = _roleManager.Roles.Select(r => r.Name).ToList();
                _cache.Set(cacheKey, roles, System.TimeSpan.FromMinutes(30));
            }
            return Ok(roles);
        }
    }
}
