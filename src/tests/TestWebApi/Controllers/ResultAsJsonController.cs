// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2023-06-07 19:29
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-07 19:29
// ***********************************************************************
//  <copyright file="ResultAsJsonController.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Models;
using AggregatedGenericResultMessage.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Models;
using TestWebApi.Services;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResultAsJsonController : ResultBaseApiController
    {
        private readonly WeatherService _weatherService;

        public ResultAsJsonController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost(nameof(JsonResultNoContent))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JsonResultNoContent(CancellationToken cancellationToken)
        {
            return JsonResult(await Task.FromResult(Result.Success()));
        }

        [HttpPost(nameof(JsonResultNoContent2))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JsonResultNoContent2(CancellationToken cancellationToken)
        {
            IResult result = new Result()
            {
                IsSuccess = true
            };

            return JsonResult(await Task.FromResult(result));
        }

        [HttpGet(nameof(JsonResult))]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JsonResult(CancellationToken cancellationToken)
        {
            return JsonResult(await _weatherService.GetCollectionAsync());
        }

        [HttpGet(nameof(WholeJsonResult))]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<MessageModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> WholeJsonResult(CancellationToken cancellationToken)
        {
            return JsonWholeResult(await _weatherService.GetCollectionAsync());
        }
    }
}