using CentralHub.Core.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralHub.Infrastructure.Data.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            // Get the RoleManager from the service provider
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Check if the "Admin" role exists
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                // Create the "Admin" role
                await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));
            }

            // Check if the "Employee" role exists
            if (!await roleManager.RoleExistsAsync(Roles.Employee))
            {
                // Create the "Employee" role
                await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Employee));
            }
        }
    }
}
