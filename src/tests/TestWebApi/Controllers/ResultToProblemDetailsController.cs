// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2024-12-25 14:34
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 14:34
// ***********************************************************************
//  <copyright file="ResultToProblemDetailsController.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using AggregatedGenericResultMessage.Enums;
using AggregatedGenericResultMessage.Extensions.Result;
using AggregatedGenericResultMessage.Models;
using AggregatedGenericResultMessage.Web.Extensions.ProblemDetail;
using Microsoft.AspNetCore.Http;
using TestWebApi.Services;

namespace TestWebApi.Controllers
{
    [ApiController]
    public class ResultToProblemDetailsController : Controller
    {
        private readonly WeatherService _weatherService;

        public ResultToProblemDetailsController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost(nameof(AsToProblemDetailsWithOk))]
        public async Task<IActionResult> AsToProblemDetailsWithOk(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionAsync();

            return res.AsProblemDetails(HttpStatusCode.OK);
        }

        [HttpPost(nameof(AsToProblemDetailsWithNoContent))]
        public async Task<IActionResult> AsToProblemDetailsWithNoContent(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultAsync();

            return res.AsProblemDetails(HttpStatusCode.NoContent);
        }

        [HttpPost(nameof(AsToProblemDetailsWithBadRequest))]
        public async Task<IActionResult> AsToProblemDetailsWithBadRequest(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();

            res.WithError(new MessageDataModel("Error message", "Error code"))
                .WithMessage(new MessageDataModel("Message info", "Message detail"), MessageType.Error);

            return res.AsProblemDetails(HttpStatusCode.BadRequest);
        }

        [HttpPost(nameof(AsToProblemDetailsWithBadRequestInstance))]
        public async Task<IActionResult> AsToProblemDetailsWithBadRequestInstance(CancellationToken cancellationToken)
        {
            var uri = HttpContext.Request.Path;
            var res = await _weatherService.GetResultFailAsync();

            res.WithError(new MessageDataModel("Error message", "Error code"))
                .WithMessage(new MessageDataModel("Message info", "Message detail"), MessageType.Error);

            return res.AsProblemDetails(HttpStatusCode.BadRequest, accessedResourceUri: uri);
        }
    }
}