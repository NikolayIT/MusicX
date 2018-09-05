namespace MusicX.Data.Seeding
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Common;
    using MusicX.Data.Models;

    public class RolesSeeder : ISeeder
    {
        public void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            SeedRole(roleManager, GlobalConstants.AdministratorRoleName);
        }

        private static void SeedRole(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            var role = roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult();
            if (role == null)
            {
                var result = roleManager.CreateAsync(new ApplicationRole(roleName)).GetAwaiter().GetResult();

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
