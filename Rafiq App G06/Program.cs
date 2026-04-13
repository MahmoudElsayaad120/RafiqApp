
using System.Threading.Tasks;
using Domain.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence;
using Persistence.Data;
using Rafiq.Api.Services;
using Rafiq_App_G06.Extensions;
using Rafiq_App_G06.Middlewares;
using Shared.ErrorModels;




namespace Rafiq_App_G06
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.RegisterAllServices(builder.Configuration);

            var app = builder.Build();





            // Configure the HTTP request pipeline.

           await app.configureMiddlewares();

            app.Run();
        }
    }
}
