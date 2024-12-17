using InternalWebsite.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Context
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ClCongDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<tblUser>>(); // Corrected this line
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Core.Entities.IdentityRole>>(); // Corrected this line if you're using GUID

            context.Database.EnsureCreated();

            // Check if any roles exist
            if (!context.Roles.Any())
            {
                // Create roles
                var roleNames = new[] { "Admin", "User","Super Admin" };
                foreach (var roleName in roleNames)
                {
                    var role = new Core.Entities.IdentityRole { Name = roleName }; // If you're using GUID
                    await roleManager.CreateAsync(role);
                }
            }

            // Check if any users exist
            if (!context.Users.Any())
            {
                // Create admin user
                var adminUser = new tblUser { UserName = "admin@web.com", Email = "admin@web.com" }; // Corrected this line
                var result = await userManager.CreateAsync(adminUser, "Password@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

    }
}
