// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApiNet8
//  Author           : RzR
//  Created On       : 2026-04-23 12:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 12:52
// ***********************************************************************
//  <copyright file="Program.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RzR.ResultMessage.Web.WebDependencyInjection;
using TestWebApiNet8.MinimalApi;
using TestWebApiNet8.ProblemDetails;
using TestWebApiNet8.Services;

#endregion

namespace TestWebApiNet8
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestWebApiNet8", Version = "v1" }));

            builder.Services.AddScoped<WeatherService>();

            // Branded ProblemDetails defaults applied globally to every Minimal-API
            // ToHttpResult / ResultMessageHttpResults.From call.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddProblemDetailsResultFactory(
                new BrandedProblemDetailsResultFactory(new HttpContextAccessor()));

            // Pipeline-wide WebResultException -> ProblemDetails handler so the /throws
            // example endpoint renders a proper response.
            builder.Services.AddResultExceptionMiddleware();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestWebApiNet8 v1"));
            }

            // Place the exception middleware before routing so middleware-level
            // exceptions are also funnelled through ProblemDetails.
            app.UseResultExceptionMiddleware();

            app.MapMinimalApiExamples();

            app.Run();
        }
    }
}