
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
using System.Text;




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



            // ????? ????? logs ?? ?? ?????
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs");

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }


            // Middleware ??? Logging
            app.Use(async (context, next) =>
            {
                var logFile = Path.Combine(logPath, $"log-{DateTime.Now:yyyy-MM-dd}.txt");

                try
                {
                    // ????? ??? Request
                    var requestLog = new StringBuilder();

                    requestLog.AppendLine("========== REQUEST ==========");
                    requestLog.AppendLine($"Time: {DateTime.Now}");
                    requestLog.AppendLine($"Method: {context.Request.Method}");
                    requestLog.AppendLine($"Path: {context.Request.Path}");
                    requestLog.AppendLine($"Query: {context.Request.QueryString}");
                    requestLog.AppendLine($"IP: {context.Connection.RemoteIpAddress}");
                    requestLog.AppendLine("=============================");
                    requestLog.AppendLine();

                    await File.AppendAllTextAsync(logFile, requestLog.ToString());

                    await next();
                }
                catch (Exception ex)
                {
                    // ????? ??? Exception
                    var errorLog = new StringBuilder();

                    errorLog.AppendLine("========== ERROR ==========");
                    errorLog.AppendLine($"Time: {DateTime.Now}");
                    errorLog.AppendLine($"Message: {ex.Message}");
                    errorLog.AppendLine($"StackTrace: {ex.StackTrace}");
                    errorLog.AppendLine("===========================");
                    errorLog.AppendLine();

                    await File.AppendAllTextAsync(logFile, errorLog.ToString());

                    throw;
                }
            });

            



            // Configure the HTTP request pipeline.

            await app.configureMiddlewares();

            app.Run();
        }
    }
}
