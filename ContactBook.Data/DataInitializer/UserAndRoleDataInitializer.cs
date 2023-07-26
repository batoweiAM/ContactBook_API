using ContactBook.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Data.DataInitializer
{
    public class UserAndRoleDataInitializer
    {
        public static async Task SeedData(ContactBookContext context, UserManager<AppUser> userManager,
               RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }
        private static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (userManager.FindByEmailAsync("torredodom@gmail.com").Result == null)
            {
                AppUser user = new AppUser
                {
                    FirstName = "Torredo",
                    LastName = "Dom",
                    Email = "torredodom@gmail.com",
                    ImageUrl = "openForCorrection",
                    FacebookUrl = "facebookurl",
                    TwitterUrl = "twitterurl",
                    UserName = "torredodom@gmail.com",
                    PhoneNumber = "2348160593167",
                    City = "Ikeja",
                    State = "Lagos",
                    Country = "Nigeria"
                };
                IdentityResult result = userManager.CreateAsync(user, "Password@1").Result;
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
            if (userManager.FindByEmailAsync("batoweim@gmail.com").Result == null)
            {
                AppUser user = new AppUser
                {
                    FirstName = "Batowei",
                    LastName = "Michael",
                    Email = "batoweim@gmail.com",
                    UserName = "batoweim@gmail.com",
                    ImageUrl = "openForCorrection",
                    FacebookUrl = "facebookurl",
                    TwitterUrl = "twitterurl",
                    PhoneNumber = "+2349018015592",
                    City = "Port Harcourt",
                    State = "Rivers",
                    Country = "Nigeria"
                };
                IdentityResult result = userManager.CreateAsync(user, "Password@2").Result;
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.RoleExistsAsync("Admin").Result == false)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };

                await roleManager.CreateAsync(role);
            }
        }
    }
}
