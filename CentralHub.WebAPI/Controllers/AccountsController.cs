using CentralHub.Application.Features.Accounts.DTOs;
using CentralHub.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CentralHub.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // POST /api/accounts/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Email already in use." });
            }

            // This is the key: we create a new TenantId for this new user
            var newTenantId = Guid.NewGuid();

            var newUser = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email, // Use email as username
                TenantId = newTenantId // Assign the new TenantId
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = string.Join(", ", errors) });
            }

    
            // For now, the user's TenantId is the "master key" for their business.

            return Ok(new AuthResponseDto { IsSuccess = true, Message = "User registered successfully." });
        }
        // POST /api/accounts/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password." });
            }

            // --- User is valid, generate token ---
            var token = GenerateJwtToken(user); // Call our new method

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = token // Return the token
            });
        }
        private string GenerateJwtToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // These are the "claims" that will be in the token.
            // This is the data our CurrentUserService will read.
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (User ID)
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID
        new Claim("tenant_id", user.TenantId.ToString()) // Our custom TenantId claim
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Token is valid for 1 hour
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
