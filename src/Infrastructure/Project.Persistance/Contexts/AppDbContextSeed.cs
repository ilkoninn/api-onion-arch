using Microsoft.AspNetCore.Identity;
using Project.Core.Entities.Identities;
using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Persistance.Contexts
{
    public static class AppDbContextSeed
    {
        public static async Task SeedDatabaseAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetValues(typeof(EUserRoles)).Cast<EUserRoles>().Select(x => x.ToString()))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminExists = await userManager.FindByNameAsync("admin");

            if (adminExists == null)
            {
                var userAdmin = new User { UserName = "admin", Email = "admin@admin.com", EmailConfirmed = true };
                await userManager.CreateAsync(userAdmin, "!Admin123.?");
                await userManager.AddToRoleAsync(userAdmin, EUserRoles.Admin.ToString());
            }

            await context.SaveChangesAsync();
        }
    }
}
