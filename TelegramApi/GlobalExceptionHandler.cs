using BOL;
using DAL.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using TelegramApi.Modals;
using static System.Net.Mime.MediaTypeNames;

namespace TelegramApi
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public static class GlobalExceptionHandler
    {
        public static void HandleExceptionByJarvisAlgo(this IApplicationBuilder app, IConfiguration configuration)
        {
            ResponseModel apiResponse = new();
            apiResponse.Message = "Global Exception Raised";
            apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;

            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = Text.Plain;
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        try
                        {
                            var optionsBuilder = new DbContextOptionsBuilder<GeneralContext>();
                            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                            var dbContext = new GeneralContext();

                            var objModel = new ExceptionLog
                            {
                                Description = "",
                                ErrorMessage = ex.Error.Message,
                                ExceptionType = "API",
                                Notes = "Route Source: " + ex.Endpoint.DisplayName,
                                StackTrace = ex.Error.StackTrace,
                                CreatedDate = DateTime.Now
                            };
                            await dbContext.ExceptionLogs.AddAsync(objModel);
                            await dbContext.SaveChangesAsync();
                            apiResponse.Data = objModel;
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
                        }
                        catch (Exception exasdASD)
                        {
                            apiResponse.Data = exasdASD;

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
                        }
                    }
                    else
                    {
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
                    }
                });
            });
        }

    }
}



 