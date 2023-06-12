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
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
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
    }
}