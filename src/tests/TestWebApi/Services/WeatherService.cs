// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2023-06-07 19:37
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-07 19:37
// ***********************************************************************
//  <copyright file="WeatherService.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RzR.ResultMessage;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Extensions.Result;
using RzR.ResultMessage.Models;
using TestWebApi.Models;

namespace TestWebApi.Services
{
    public class WeatherService
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task<IResult<IEnumerable<WeatherForecast>>> GetCollectionAsync()
        {
            var rng = new Random();
            var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
                .ToArray();

            return await Task.FromResult(Result<IEnumerable<WeatherForecast>>.Success(data));
        }

        public async Task<Result<IEnumerable<WeatherForecast>>> GetCollection1Async()
        {
            try
            {
                var rng = new Random();
                var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                    .ToArray();

                return await Task.FromResult(Result<IEnumerable<WeatherForecast>>.Success(data));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public async Task<Result> GetResultAsync()
        {
            var result = new Result { IsSuccess = true };

            return await Task.FromResult(result);
        }

        public async Task<Result> GetResultFailAsync()
        {
            var result = new Result { IsSuccess = false };

            return await Task.FromResult(result);
        }

        public async Task<IResult> GetResultMultiFailAsync()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new MessageDataModel("First validation error", "E001-First validation error"), "c-E001");
            result.WithError(new MessageDataModel("Second validation error", "E002-Second validation error"), "c-E002");
            result.WithError(new MessageDataModel("Third validation error", "E003-Third validation error"));

            return await Task.FromResult(result);
        }

        public async Task<IResult<IEnumerable<WeatherForecast>>> GetCollectionMultiFailAsync()
        {
            var result = Result<IEnumerable<WeatherForecast>>.Failure("x-c-e000", "Forecast service unavailable");
            result.WithError(new MessageDataModel("Upstream timed out", "E100-Upstream timed out"));
            result.WithError(new MessageDataModel("Cache miss", "E101-Cache miss", "Cache error!"));

            return await Task.FromResult(result);
        }
    }
}