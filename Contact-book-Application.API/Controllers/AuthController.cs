using Contact_book_Application.API.DTOs;
using ContactBook.Auth;
using ContactBook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Contact_book_Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        public AuthController(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var Password = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!Password)
            {
                return BadRequest("Invalid credentials");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var UserRoles = roles.ToArray();

            var token = TokenGenerator.GenerateToken(model.Email, user.Id, model.Password, _config, UserRoles);

            return Ok(token);
        }
    }
}
