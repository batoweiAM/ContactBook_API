using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Contact_book_Application.API.DTOs;
using ContactBook.Data;
using ContactBook.Models;
using ContactBook.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;

namespace Contact_book_Application.API.Repository
{
    public class Repository : IRepository
    {
        private readonly ContactBookContext _contactBookContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly Cloudinary _cloudinary;

        public Repository(ContactBookContext contactBookContext, UserManager<AppUser> userManager, Cloudinary cloudinary)
        {
            _contactBookContext = contactBookContext;
            _userManager = userManager;
            _cloudinary = cloudinary;
        }

        public IEnumerable<AppUserDTO> GetAllUsers(PaginParameter usersParameter)
        {
            var utility = new Utilities(_contactBookContext);
            var data = new List<AppUserDTO>();
            foreach (var userdata in utility.GetAllUsers(usersParameter))
            {
                data.Add(new AppUserDTO
                {
                    FirstName = userdata.FirstName,
                    LastName = userdata.LastName,
                    Email = userdata.Email,
                    PhoneNumber = userdata.PhoneNumber,
                    ImageUrl = userdata.ImageUrl,
                    FacebookUrl = userdata.FacebookUrl,
                    TwitterUrl = userdata.TwitterUrl,
                    City = userdata.City,
                    State = userdata.State,
                    Country = userdata.Country
                });
            }
            return data;
        }

        public async Task<AppUser> GetByIdAsync(string id)
        {
            var userdata = await _contactBookContext.appUsers.FindAsync(id);
            if (userdata == null)
            {
                return null;
            }

            var data = new AppUser
            {
                FirstName = userdata.FirstName,
                LastName = userdata.LastName,
                Email = userdata.Email,
                PhoneNumber = userdata.PhoneNumber,
                ImageUrl = userdata.ImageUrl,
                FacebookUrl = userdata.FacebookUrl,
                TwitterUrl = userdata.TwitterUrl,
                City = userdata.City,
                State = userdata.State,
                Country = userdata.Country
            };
            return data;
        }


        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var userdata = await _userManager.FindByEmailAsync(email);
            if (userdata == null)
            {
                return new BadRequestResult();
            }

            var data = new AppUserDTO
            {
                FirstName = userdata.FirstName,
                LastName = userdata.LastName,
                Email = userdata.Email,
                PhoneNumber = userdata.PhoneNumber,
                ImageUrl = userdata.ImageUrl,
                FacebookUrl = userdata.FacebookUrl,
                TwitterUrl = userdata.TwitterUrl,
                City = userdata.City,
                State = userdata.State,
                Country = userdata.Country
            };

            return new OkObjectResult(data);
        }

        public async Task<IActionResult> AddNewUserAsync(AppUserDTO appUser)
        {
            var user = await _userManager.FindByEmailAsync(appUser.Email);
            if (user == null)
            {
                var data = new AppUser()
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    UserName = appUser.Email,
                    Email = appUser.Email,
                    ImageUrl = appUser.ImageUrl,
                    PhoneNumber = appUser.PhoneNumber,
                    FacebookUrl = appUser.FacebookUrl,
                    TwitterUrl = appUser.TwitterUrl,
                    City = appUser.City,
                    State = appUser.State,
                    Country = appUser.Country
                };

                var res = await _userManager.CreateAsync(data, appUser.Password);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(data, "User");
                    return new OkObjectResult("User created successfully");
                }
            }

            return new BadRequestObjectResult("User already exists");
        }


        public async Task<IActionResult> UpdateUserAsync(string id, AppUserDTO appUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var pas = _userManager.PasswordHasher.HashPassword(user, appUser.Password);
                user.FirstName = appUser.FirstName;
                user.LastName = appUser.LastName;
                user.UserName = appUser.Email;
                user.Email = appUser.Email;
                user.ImageUrl = appUser.ImageUrl;
                user.PasswordHash = pas;
                user.PhoneNumber = appUser.PhoneNumber;
                user.FacebookUrl = appUser.FacebookUrl;
                user.TwitterUrl = appUser.TwitterUrl;
                user.City = appUser.City;
                user.State = appUser.State;
                user.Country = appUser.Country;
                var res = await _userManager.UpdateAsync(user);
                return new OkObjectResult(res);
            }
            return new BadRequestObjectResult("User not found");
        }

        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new BadRequestObjectResult("Failed to delete user");
            }

            _contactBookContext.SaveChanges();
            return new OkObjectResult("User successfully removed");
        }

        public async Task<IActionResult> AddUserPhotoAsync(string id, PhotoToAddDTO model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found");
            }

            if (id != user.Id)
            {
                return new UnauthorizedResult();
            }

            var file = model.PhotoFile;
            if (file.Length <= 0)
            {
                return new BadRequestObjectResult("Invalid file size");
            }

            var imageUploadResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, fs),
                    Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")
                };
                imageUploadResult = _cloudinary.Upload(imageUploadParams);
            }

            var publicId = imageUploadResult.PublicId;
            var url = imageUploadResult.Url.AbsolutePath;
            user.ImageUrl = url;
            await _userManager.UpdateAsync(user);

            return new OkObjectResult(new { Id = publicId, Url = url });
        }

        public async Task<IActionResult> SearchUsersAsync(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return new BadRequestObjectResult("Search term cannot be empty");
            }

            var users = await _userManager.Users
                .Where(u => u.Email.Contains(term)
                            || u.FirstName.Contains(term)
                            || u.LastName.Contains(term)
                            || u.City.Contains(term)
                            || u.State.Contains(term)
                            || u.Country.Contains(term))
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return new NotFoundObjectResult("Search Result Empty");
            }

            var appUserDTOs = users.Select(item => new AppUserDTO()
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                PhoneNumber = item.PhoneNumber,
                ImageUrl = item.ImageUrl,
                FacebookUrl = item.FacebookUrl,
                TwitterUrl = item.TwitterUrl,
                City = item.City,
                State = item.State,
                Country = item.Country
            }).ToList();

            return new OkObjectResult(appUserDTOs);
        }
    }
}

