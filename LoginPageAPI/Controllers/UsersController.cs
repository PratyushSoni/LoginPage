using LoginPageAPI.DTOs;
using LoginPageAPI.Models;
using LoginPageAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginPageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _cache;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMemoryCache cache, ITokenService tokenService, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cache = cache;
            _tokenService = tokenService;
            _logger = logger;
        }

        // POST: api/users/create
        // 🔒 Admin-only
        [HttpPost("create")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required");

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Optionally add role if provided
            if (!string.IsNullOrEmpty(dto.Role))
            {
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            var response = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };

            // Remove user cache if exists
            _cache.Remove($"user:{user.UserName}");

            return Ok(response);
        }

        // GET: api/users/{username}
        // 🔒 Authenticated users can view their details
        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string username)
        {
            var cacheKey = $"user:{username}";
            if (!_cache.TryGetValue<UserDto>(cacheKey, out var response))
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null) return NotFound();

                response = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                };

                _cache.Set(cacheKey, response, System.TimeSpan.FromMinutes(10));
            }
            return Ok(response);
        }

        // GET: api/users/check?username=...&email=...
        [HttpGet("check")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string username, [FromQuery] string email)
        {
            try
            {
                bool usernameExists = false;
                bool emailExists = false;

                if (!string.IsNullOrWhiteSpace(username))
                    usernameExists = await _userManager.FindByNameAsync(username) != null;

                if (!string.IsNullOrWhiteSpace(email))
                    emailExists = await _userManager.FindByEmailAsync(email) != null;

                return Ok(new { usernameExists, emailExists });
            }
            catch (System.Exception ex)
            {
                // Log the exception (for now, return it in the response for debugging)
                return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        // GET: api/users/all
        // 🔒 Admin/Manager only
        [HttpGet("all")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }
            return Ok(userList);
        }
    }
}
