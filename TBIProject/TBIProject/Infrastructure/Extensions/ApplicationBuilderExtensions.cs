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

                    var adminName = "manager@manager.com";

                    var adminUser = await userManager.FindByEmailAsync(adminName);

                    if (adminUser == null)
                    {
                        adminUser = new User
                        {
                            UserName = adminName,
                            Email = "manager@manager.com"
                        };

                        await userManager.CreateAsync(adminUser, "manager12");
                        await userManager.AddToRoleAsync(adminUser, managerRole);
                    }

                })
                .GetAwaiter()
                .GetResult();
            }
        }
    }
}
