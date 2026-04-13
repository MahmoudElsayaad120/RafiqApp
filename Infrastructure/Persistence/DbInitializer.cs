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
                //Data Seeding
                //seeding users from JSON files

                if (!context.Users.Any())
                {
                    //1.read all data from types file as string
                    var userData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\Users.json");

                    //2.transform string to C# objects [list<users>]
                    var users = JsonSerializer.Deserialize<List<User>>(userData);

                    //3.add list<users> to database
                    if (users is not null && users.Any())
                    {

                        await context.Users.AddRangeAsync(users);
                        await context.SaveChangesAsync();



                    }
                }





                //seeding doctors from JSON files

                if (!context.Doctors.Any())
                {
                    //1.read all data from types file as string
                    var doctorData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\Doctors.json");

                    //2.transform string to C# objects [list<doctors>]
                    var doctors = JsonSerializer.Deserialize<List<Doctor>>(doctorData);

                    //3.add list<doctors> to database
                    if (doctors is not null && doctors.Any())
                    {
                        await context.Doctors.AddRangeAsync(doctors);
                        await context.SaveChangesAsync();
                    }
                }





                //seeding patients from JSON files

                if (!context.Patients.Any())
                {
                    //1.read all data from types file as string
                    var patientData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\Patients.json");

                    //2.transform string to C# objects [list<patient>]
                    var patients = JsonSerializer.Deserialize<List<Patient>>(patientData);

                    //3.add list<patient> to database
                    if (patients is not null && patients.Any())
                    {
                        await context.Patients.AddRangeAsync(patients);
                        await context.SaveChangesAsync();
                    }
                }




                //seeding DoctorAvailabilities from JSON files

                if (!context.DoctorAvailabilities.Any())
                {
                    //1.read all data from types file as string
                    var DoctorAvailabilitieData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\DoctorAvailabilities.json");

                    //2.transform string to C# objects [list<DoctorAvailabilitie>]
                    var DoctorAvailabilities = JsonSerializer.Deserialize<List<DoctorAvailability>>(DoctorAvailabilitieData);

                    //3.add list<DoctorAvailabilitie> to database
                    if (DoctorAvailabilities is not null && DoctorAvailabilities.Any())
                    {
                        await context.DoctorAvailabilities.AddRangeAsync(DoctorAvailabilities);
                        await context.SaveChangesAsync();
                    }
                }



                //seeding ChatMessages from JSON files

                if (!context.ChatMessages.Any())
                {
                    //1.read all data from types file as string
                    var chatmessageData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\ChatMessages.json");

                    //2.transform string to C# objects [list<ChatMessages>]
                    var ChatMessages = JsonSerializer.Deserialize<List<ChatMessage>>(chatmessageData);

                    //3.add list<ChatMessages> to database
                    if (ChatMessages is not null && ChatMessages.Any())
                    {
                        await context.ChatMessages.AddRangeAsync(ChatMessages);
                        await context.SaveChangesAsync();
                    }
                }



                //seeding Appointments from JSON files

                if (!context.Appointments.Any())
                {
                    //1.read all data from types file as string
                    var appointmentData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\Appointments.json");

                    //2.transform string to C# objects [list<Appointments>]
                    var Appointments = JsonSerializer.Deserialize<List<Appointment>>(appointmentData);

                    //3.add list<Appointments> to database
                    if (Appointments is not null && Appointments.Any())
                    {
                        await context.Appointments.AddRangeAsync(Appointments);
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
