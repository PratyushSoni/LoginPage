using LoginPageAPI.DTOs;
using LoginPageAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LoginPageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _cache;

        public UsersController(UserManager<ApplicationUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager, IMemoryCache cache)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
            _cache = cache;
        }

        // Generate JWT token with roles
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var token = await GenerateJwtToken(user);
                return Ok(new
                {
                    token,
                    username = user.UserName,
                    email = user.Email,
                    roles = await _userManager.GetRolesAsync(user)
                });
            }

            return Unauthorized("Invalid username or password");
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
