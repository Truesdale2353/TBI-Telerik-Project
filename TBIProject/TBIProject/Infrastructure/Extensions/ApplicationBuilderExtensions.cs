using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBIProject.Data;
using TBIProject.Data.Models;

namespace TBIProject.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UpdateDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<TBIContext>();
                context.Database.Migrate();

                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();

                Task.Run(async () =>
                {
                    var managerRole = "Manager";

                    var exists = await roleManager.RoleExistsAsync(managerRole);

                    if (!exists)
                    {
                        await roleManager.CreateAsync(new IdentityRole
                        {
                            Name = managerRole
                        });
                    }

                    var operatorRole = "Operator";

                    var operatorRoleExists = await roleManager.RoleExistsAsync(operatorRole);

                    if (!operatorRoleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole
                        {
                            Name = operatorRole
                        });
                    }

                    var managerName = "manager@manager.com";

                    var managerUser = await userManager.FindByEmailAsync(managerName);

                    if (managerUser == null)
                    {
                        managerUser = new User
                        {
                            UserName = managerName,
                            Email = "manager@manager.com"
                        };

                        await userManager.CreateAsync(managerUser, "manager12");
                        await userManager.AddToRoleAsync(managerUser, managerRole);
                    }

                    var adminRole = "Admin";

                    var adminRoleExists = await roleManager.RoleExistsAsync(adminRole);

                    if (!adminRoleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole
                        {
                            Name = adminRole
                        });
                    }

                    var adminName = "admin@admin.com";

                    var adminUser = await userManager.FindByEmailAsync(adminName);

                    if (adminUser == null)
                    {
                        adminUser = new User
                        {
                            UserName = adminName,
                            Email = "admin@admin.com"
                        };

                        await userManager.CreateAsync(adminUser, "admin12");
                        await userManager.AddToRoleAsync(adminUser, adminRole);
                    }

                })
                .GetAwaiter()
                .GetResult();
            }
        }
    }
}
