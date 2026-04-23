// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApiNet8
//  Author           : RzR
//  Created On       : 2026-04-23 12:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 12:52
// ***********************************************************************
//  <copyright file="WeatherService.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RzR.ResultMessage;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Extensions.Result;
using RzR.ResultMessage.Models;
using TestWebApiNet8.Models;

#endregion

namespace TestWebApiNet8.Services
{
    public class WeatherService
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild",
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<IResult<IEnumerable<WeatherForecast>>> GetCollectionAsync()
        {
            var rng = new Random();
            var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray();

            return Task.FromResult<IResult<IEnumerable<WeatherForecast>>>(
                Result<IEnumerable<WeatherForecast>>.Success(data));
        }

        public Task<Result> GetEmptySuccessAsync()
        {
            return Task.FromResult(new Result { IsSuccess = true });
        }

        public Task<IResult> GetMultiFailAsync()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new MessageDataModel("First validation error", "E001"), "c-E001");
            result.WithError(new MessageDataModel("Second validation error", "E002"), "c-E002");
            result.WithError(new MessageDataModel("Third validation error", "E003"));

            return Task.FromResult<IResult>(result);
        }

        public Task<IResult<IEnumerable<WeatherForecast>>> GetCollectionMultiFailAsync()
        {
            var result = Result<IEnumerable<WeatherForecast>>.Failure("x-c-e000", "Forecast service unavailable");
            result.WithError(new MessageDataModel("Upstream timed out", "E100"));
            result.WithError(new MessageDataModel("Cache miss", "E101", "Cache error!"));

            return Task.FromResult<IResult<IEnumerable<WeatherForecast>>>(result);
        }
    }
}