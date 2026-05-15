using Domain.Exceptions;
using Shared.ErrorModels;

namespace Rafiq_App_G06.Middlewares
{
    public class GlobalErrorHandingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalErrorHandingMiddleware> logger;

        public GlobalErrorHandingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            //try
            //{
            //    await next.Invoke(context);
            //    if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            //    {
            //        await HandingNotFoundEndPointAsync(context);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, ex.Message);

            //    await HandlingErrorAsync(context, ex);
            //}

            try
            {
                await next.Invoke(context);

                // بنتحقق إن الرد لم يبدأ بعد قبل معالجة الـ 404
                if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    await HandingNotFoundEndPointAsync(context);
                }
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ في الـ Log عشان نعرف سببه الحقيقي من الـ Console
                logger.LogError(ex, ex.Message);

                // لو السيرفر بدأ يبعت البيانات فعلاً، مينفعش نعدل الهيدرز أو نبعت رد جديد
                if (context.Response.HasStarted)
                {
                    return;
                }

                // لو لسه مبعتناش حاجة، نبعت الـ Error الـ JSON العادي بتاعنا
                await HandlingErrorAsync(context, ex);
            }

        }

        private static async Task HandlingErrorAsync(HttpContext context, Exception ex)
        {
            //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ErrorDetails()
            {
                //statusCode= StatusCodes.Status500InternalServerError,
                ErrorMessage = ex.Message
            };

            response.statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = response.statusCode;

            await context.Response.WriteAsJsonAsync(response);
        }

        private static async Task HandingNotFoundEndPointAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorDetails()
            {
                //statusCode= StatusCodes.Status500InternalServerError,
                statusCode = StatusCodes.Status404NotFound,
                ErrorMessage = $"End Point {context.Request.Path} is not found"
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
