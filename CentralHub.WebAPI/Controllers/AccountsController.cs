using CentralHub.Application.Features.Accounts.DTOs;
using CentralHub.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CentralHub.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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


            // For now, a successful login just returns a 200 OK.
            return Ok(new AuthResponseDto { IsSuccess = true, Message = "Login successful." });
        }
    }
}
