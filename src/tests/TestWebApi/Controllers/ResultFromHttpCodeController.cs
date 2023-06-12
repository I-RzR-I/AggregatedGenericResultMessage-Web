// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2023-06-09 15:30
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-09 17:07
// ***********************************************************************
//  <copyright file="ResultFromHttpCodeController.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.ActionResult;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Services;
// ReSharper disable InconsistentNaming

#endregion

namespace TestWebApi.Controllers
{
    [ApiController]
    public class ResultFromHttpCodeController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public ResultFromHttpCodeController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost(nameof(ResultAsActionResultCode204))]
        public IActionResult ResultAsActionResultCode204(CancellationToken cancellationToken)
        {
            var res = Result.Success();

            return res.AsActionResult(HttpStatusCode.NoContent);
        }

        [HttpPost(nameof(ResultAsActionResultCode200))]
        public IActionResult ResultAsActionResultCode200(CancellationToken cancellationToken)
        {
            var res = Result<bool>.Success(true);

            return res.AsActionResult(HttpStatusCode.OK);
        }

        [HttpPost(nameof(IResultAsActionResultCode204))]
        public IActionResult IResultAsActionResultCode204(CancellationToken cancellationToken)
        {
            IResult res = Result.Success();

            return res.AsActionResult(HttpStatusCode.NoContent);
        }

        [HttpPost(nameof(IResultAsActionResultCode200))]
        public IActionResult IResultAsActionResultCode200(CancellationToken cancellationToken)
        {
            IResult<bool> res = Result<bool>.Success(true);

            return res.AsActionResult(HttpStatusCode.OK);
        }

        [HttpPost(nameof(IResultAsActionResultCode200x1))]
        public async Task<IActionResult> IResultAsActionResultCode200x1(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionAsync();

            return res.AsActionResult(HttpStatusCode.OK);
        }

        [HttpPost(nameof(ResultAsActionResultCode200x2))]
        public async Task<IActionResult> ResultAsActionResultCode200x2(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollection1Async();

            return res.AsActionResult(HttpStatusCode.OK);
        }

        [HttpPost(nameof(ResultAsActionResultCode204x1))]
        public async Task<IActionResult> ResultAsActionResultCode204x1(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultAsync();

            return res.AsActionResult(HttpStatusCode.NoContent);
        }

        [HttpPost(nameof(ResultFailureAsActionResultCode204x2AnywaySuccess))]
        public async Task<IActionResult> ResultFailureAsActionResultCode204x2AnywaySuccess(
            CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();

            return res.AsActionResult(HttpStatusCode.NoContent);
        }
    }
}