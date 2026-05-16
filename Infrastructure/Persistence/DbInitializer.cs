using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Identity;

namespace Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RafiqDbContext context;
        private readonly RafiqIdentityDbContext identityDbContext;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbInitializer(
            RafiqDbContext context,
            RafiqIdentityDbContext identityDbContext,
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager
            )
        {
            this.context = context;
            this.identityDbContext = identityDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task InitializeAsync()
        {
            try 
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                }


                if (!context.Specializations.Any())
                {
                    // Read json file
                    var specializationData = await File.ReadAllTextAsync(
                        @"..\Infrastructure\Persistence\Data\Seeding\Specializations.json");

                    // Convert json to objects
                    var specializations = JsonSerializer.Deserialize<List<Specialization>>(specializationData);

                    // Save to database
                    if (specializations is not null && specializations.Any())
                    {
                        await context.Specializations.AddRangeAsync(specializations);
                        await context.SaveChangesAsync();
                    }
                }



            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task InitializeIdentityAsync()
        {
            if (identityDbContext.Database.GetPendingMigrations().Any()) 
            {
                await identityDbContext.Database.MigrateAsync();
            }


            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole() 
                {
                    Name = "Admin"
                });
                await roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "SuperAdmin"
                });
            }
                

            //Seeding
            if (!userManager.Users.Any())
            {
                var superAdminUser = new AppUser()
                {
                    DisplayName = "Super Admin",
                    Email = "SuperAdmin@gmail.com",
                    UserName = "SuperAdmin", 
                    PhoneNumber = "01014756248"
                };
                var AdminUser = new AppUser()
                {
                    DisplayName = "Admin",
                    Email = "Admin@gmail.com",
                    UserName = "Admin",
                    PhoneNumber = "01014756248"
                };

               await  userManager.CreateAsync(superAdminUser, "P@ssW0rd");
                await userManager.CreateAsync(AdminUser, "P@ssW0rd");



                await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                await userManager.AddToRoleAsync(AdminUser, "Admin");


            }

        }
    }
}
