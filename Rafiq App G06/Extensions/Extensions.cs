using System.Text;
using Domain.Contracts;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Persistence.Data;
using Persistence.Identity;
using Rafiq.Api.Services.Abstractions;
using Rafiq_App_G06.Middlewares;
using Services;
using Shared;
using Shared.ErrorModels;

namespace Rafiq_App_G06.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddBuiltInServices();
            services.AddSwaggerServices();
            services.ConfigureServices();



            services.AddDbContext<RafiqDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            });
            services.AddScoped<IDbInitializer, DbInitializer>();//allow di for DbInitializer
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddInfrastructureservices(configuration);
            services.AddIdentityServices();
            services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IDoctorService, DoctorService>();
            //services.AddAutoMapper();
            services.ConfigureJwtServices(configuration);

            services.AddCors(config => 
            {
                config.AddPolicy("MyPolicy", options => 
                {
                    options.WithOrigins()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });


            return services;
        }

        private static IServiceCollection AddBuiltInServices(this IServiceCollection services)
        {

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddControllers();


            return services;
        }
        private static IServiceCollection ConfigureJwtServices(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });


            return services;
        }

        private static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<RafiqIdentityDbContext>();
     
            return services;
        }


        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        private static IServiceCollection ConfigureServices(this IServiceCollection services)
        {

            services.Configure<ApiBehaviorOptions>(config =>
            {
                config.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(m => m.Value.Errors.Any())
                                 .Select(m => new ValidationError()
                                 {
                                     Field = m.Key,
                                     Errors = m.Value.Errors.Select(errors => errors.ErrorMessage)
                                 });

                    var response = new ValidationErrorResponsse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult("");
                };

            });

            return services;
        }


        public static async Task<WebApplication> configureMiddlewares(this WebApplication app)
        {
           await app.InitializeDatabaseAsync();
            app.UseGlobalErrorHanding();


            //Code


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }


        private static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
        {
            #region Seeding

            // Configure the HTTP request pipeline.
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();//ask clr create object from DbInitializer
            await dbInitializer.InitializeAsync();
            await dbInitializer.InitializeIdentityAsync();

            #endregion

            return app;

        }

        private static WebApplication UseGlobalErrorHanding(this WebApplication app)
        {

            app.UseMiddleware<GlobalErrorHandingMiddleware>();

            return app;

        }
    }
}
