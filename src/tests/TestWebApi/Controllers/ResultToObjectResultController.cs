// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2023-06-11 20:32
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-11 20:32
// ***********************************************************************
//  <copyright file="ResultToObjectResultController.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.ActionResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Services;

namespace TestWebApi.Controllers
{
    [ApiController]
    public class ResultToObjectResultController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public ResultToObjectResultController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost(nameof(AsSuccessObjectResultx0))]
        public async Task<IActionResult> AsSuccessObjectResultx0(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultAsync();

            return res.AsSuccessObjectResult();
        }

        [HttpPost(nameof(AsSuccessObjectResultx01))]
        public async Task<IActionResult> AsSuccessObjectResultx01(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();

            return res.AsSuccessObjectResult();
        }

        [HttpPost(nameof(AsSuccessObjectResultx1))]
        public IActionResult AsSuccessObjectResultx1(CancellationToken cancellationToken)
        {
            IResult res = Result.Success();

            return res.AsSuccessObjectResult();
        }

        [HttpPost(nameof(AsSuccessObjectResultx2))]
        public async Task<IActionResult> AsSuccessObjectResultx2(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollection1Async();

            return res.AsSuccessObjectResult(StatusCodes.Status200OK);
        }

        [HttpPost(nameof(AsSuccessObjectResultx3))]
        public async Task<IActionResult> AsSuccessObjectResultx3(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollection1Async();

            return res.AsSuccessObjectResult(HttpStatusCode.OK);
        }

        [HttpPost(nameof(AsSuccessObjectResultx4))]
        public async Task<IActionResult> AsSuccessObjectResultx4(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionAsync();

            return res.AsSuccessObjectResult(StatusCodes.Status200OK);
        }

        [HttpPost(nameof(AsSuccessObjectResultx5))]
        public async Task<IActionResult> AsSuccessObjectResultx5(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionAsync();

            return res.AsSuccessObjectResult(HttpStatusCode.OK);
        }
    }
}