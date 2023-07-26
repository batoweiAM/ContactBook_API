using Contact_book_Application.API.DTOs;
using ContactBook.Models;
using ContactBook.Services;
using Microsoft.AspNetCore.Mvc;

namespace Contact_book_Application.API.Repository
{
    public interface IRepository
    {
        IEnumerable<AppUserDTO> GetAllUsers(PaginParameter usersParameter);
        Task<AppUser> GetByIdAsync(string id);
        Task<IActionResult> GetUserByEmailAsync(string email);
        Task<IActionResult> AddNewUserAsync(AppUserDTO appUser);
        Task<IActionResult> UpdateUserAsync(string id, AppUserDTO appUser);
        Task<IActionResult> DeleteUserAsync(string id);
        Task<IActionResult> AddUserPhotoAsync(string id, PhotoToAddDTO model);
        Task<IActionResult> SearchUsersAsync(string term);
    }
}
