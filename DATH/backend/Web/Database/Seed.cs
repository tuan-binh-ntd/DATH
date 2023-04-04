using Entities;
using Entities.Enum.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Customer"},
                new AppRole{Name = "Employee"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            //foreach (var user in users)
            //{
            //    user.UserName = user.UserName!.ToLower();
            //    await userManager.CreateAsync(user, "Pa$$w0rd");
            //    await userManager.AddToRoleAsync(user, "Member");
            //}

            var admin = new AppUser()
            {
                UserName = "admin",
                Type = UserType.Admin,
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Employee" });
        }
    }

}
