using Entities;
using Entities.Enum.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class Seed
    {
        public static async Task SeedUsers(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            DataContext dataContext)
        {

            //Create Parent Warehouse
            if (await dataContext.Warehouse.AnyAsync()) return;

            Warehouse parentWarehouse = new()
            {
                Name = "ParentWarehouse",
                CreationTime = DateTime.Now,
                CreatorUserId = 1
            };

            await dataContext.Warehouse.AddAsync(parentWarehouse);
            await dataContext.SaveChangesAsync();



            if (await userManager.Users.AnyAsync()) return;

            // Create role of system
            var roles = new List<AppRole>
            {
                new AppRole{ Name = "Admin" },
                new AppRole{ Name = "Customer" },
                new AppRole{ Name = "Employee" },
                new AppRole{ Name = "OrderTransfer" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // Create Addmin Account
            AppUser admin = new()
            {
                UserName = "admin",
                Type = UserType.Admin,
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Employee", "OrderTransfer" });

            // Create Order Transfer
            AppUser orderTransfer = new()
            {
                UserName = "ordertransfer",
                Type = UserType.OrderTransfer,
            };

            await userManager.CreateAsync(orderTransfer, "Abcd1234");
            await userManager.AddToRolesAsync(orderTransfer, new[] { "Admin", "Employee", "OrderTransfer" });
        }
    }

}
