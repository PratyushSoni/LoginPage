using LoginPageAPI.DTOs;
using LoginPageAPI.Models;
using LoginPageAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

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

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
            }
            return Ok(response);
        }

        // POST: api/users/login
        // 🔓 Public endpoint (no auth needed)
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user '{Username}' not found.", dto.Username);
                return Unauthorized("Invalid username or password");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: invalid password for user '{Username}'.", dto.Username);
                return Unauthorized("Invalid username or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateJwtTokenAsync(user, roles);
            return Ok(new
            {
                token,
                username = user.UserName,
                email = user.Email,
                roles
            });
        }

        // POST: api/users/assignrole
        // 🔒 Only Admin can assign roles
        [HttpPost("assignrole")]
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

        // GET: api/users/roles
        // 🔒 Admin-only
        [HttpGet("roles")]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult GetRoles()
        {
            var cacheKey = "roles:list";
            if (!_cache.TryGetValue<List<string>>(cacheKey, out var roles))
            {
                roles = _roleManager.Roles.Select(r => r.Name).ToList();
                _cache.Set(cacheKey, roles, TimeSpan.FromMinutes(30));
            }
            return Ok(roles);
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
            catch (Exception ex)
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
