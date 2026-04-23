// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApiNet8
//  Author           : RzR
//  Created On       : 2026-04-23 12:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 12:53
// ***********************************************************************
//  <copyright file="MinimalApiEndpoints.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RzR.ResultMessage;
using RzR.ResultMessage.Extensions.Result;
using RzR.ResultMessage.Models;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Extensions.MinimalApi;
using TestWebApiNet8.Services;

#endregion

namespace TestWebApiNet8.MinimalApi
{
    public static class MinimalApiEndpoints
    {
        public static IEndpointRouteBuilder MapMinimalApiExamples(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/minimal-api").WithTags("MinimalApi");

            group.MapGet("/forecasts", async (WeatherService weather) =>
                {
                    var result = await weather.GetCollectionAsync();

                    return result.ToHttpResult();
                })
                .WithName("Forecasts_Success");

            group.MapGet("/empty", async (WeatherService weather) =>
                {
                    var result = await weather.GetEmptySuccessAsync();

                    return result.ToHttpResult();
                })
                .WithName("Empty_NoContent");

            group.MapGet("/validation", async (WeatherService weather, HttpContext http) =>
                {
                    var result = await weather.GetMultiFailAsync();

                    return result.ToHttpResult(httpContext: http);
                })
                .WithName("Validation_BadRequest");

            group.MapGet("/orders/{id:int}", (int id, HttpContext http) =>
                {
                    var result = new Result { IsSuccess = false }
                        .WithError(new MessageDataModel("Order not found", "E404"), "E404");

                    return result.ToHttpResult(
                        statusCode: HttpStatusCode.NotFound,
                        message: "Order not found",
                        detailMessage: $"No order matched id '{id}'.",
                        accessedResourceUri: http.Request.Path,
                        httpContext: http);
                })
                .WithName("Orders_NotFound");

            group.MapGet("/from-facade", async (WeatherService weather, HttpContext http) =>
                {
                    var result = await weather.GetCollectionMultiFailAsync();

                    return ResultMessageHttpResults.From(result, httpContext: http);
                })
                .WithName("Facade_From");

            group.MapGet("/throws", http =>
                {
                    var result = new Result { IsSuccess = false }
                        .WithError(new MessageDataModel("Conflict", "E409-duplicate"));

                    throw new WebResultException(
                        result,
                        HttpStatusCode.Conflict,
                        "Already exists",
                        "An order with this id already exists.",
                        http.Request.Path);

#pragma warning disable CS0162 // Unreachable - present only for type inference of the lambda.
                    return Task.CompletedTask;
#pragma warning restore CS0162
                })
                .WithName("Throws_WebResultException");

            return endpoints;
        }
    }
}