using LoginPageAPI.DTOs;
using LoginPageAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public UsersController(UserManager<ApplicationUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
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

            return Ok(response);
        }

        // GET: api/users/{username}
        // 🔒 Authenticated users can view their details
        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            var response = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };

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
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }
    }
}
