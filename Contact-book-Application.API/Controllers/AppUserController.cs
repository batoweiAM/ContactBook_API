using CloudinaryDotNet;
using Contact_book_Application.API.DTOs;
using ContactBook.Data;
using ContactBook.Models;
using ContactBook.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using static System.Net.WebRequestMethods;
using System.Dynamic;
using Contact_book_Application.API.Repository;

namespace Contact_book_Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class AppUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ContactBookContext _contactBookContext;
        private readonly Cloudinary _cloudinary;

        private readonly IRepository _Repository;

        public AppUserController(IRepository Repository, IConfiguration configuration, UserManager<AppUser> userManager, ContactBookContext contactBookContext)
        {
            _Repository = Repository;
            _configuration = configuration;
            _userManager = userManager;
            _contactBookContext = contactBookContext;
            Account account = new Account 
            {
                Cloud = _configuration.GetSection("CloudinarySettings:CloudName").Value,
                ApiKey = _configuration.GetSection("CloudinarySettings:Apikey").Value,
                ApiSecret = _configuration.GetSection("CloudinarySettings:ApiSecret").Value,
            };
            _cloudinary = new Cloudinary(account);
        }

        // 
        [HttpGet("all-users")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<AppUserDTO> GetAll([FromQuery] PaginParameter usersParameter)
        {
            var data = _Repository.GetAllUsers(usersParameter);
            return data;
        }

        // 
        [HttpGet("get-user/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string Id)
        {
            var data = await _Repository.GetByIdAsync(Id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        //
        [HttpGet]
        [Route("email")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetByEmail([FromQuery] string Email)
        {
            var data = await _Repository.GetUserByEmailAsync(Email);
            return data;
        }

        //
        [HttpPost]
        [Route("add-new")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] AppUserDTO appUser)
        {
            var data = await _Repository.AddNewUserAsync(appUser);
            return data;
        }

        //
        [HttpPut]
        [Route("update/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(string id, [FromBody] AppUserDTO appUser)
        {
            var result = await _Repository.UpdateUserAsync(id, appUser);
            return result;
        }

        //
        [HttpDelete("delete/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _Repository.DeleteUserAsync(id);
            return result;
        }

        //
        [HttpPatch("photo/{Id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> AddUserPhotoAsync(string id, [FromForm] PhotoToAddDTO model)
        {
            var result = await _Repository.AddUserPhotoAsync(id, model);
            return result;
        }

        //
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var result = await _Repository.SearchUsersAsync(term);
            return result;
        }
    }
}
