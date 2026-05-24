// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 08:19
// ***********************************************************************
//  <copyright file="ResultExceptionController.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage;
using RzR.ResultMessage.Enums;
using RzR.ResultMessage.Extensions.Result;
using RzR.ResultMessage.Models;
using RzR.ResultMessage.Web.Exceptions;
using TestWebApi.Services;

#endregion

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("api/result-exception")]
    public class ResultExceptionController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public ResultExceptionController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost("throw-default")]
        public async Task<IActionResult> ThrowDefault(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();
            res.WithError(new MessageDataModel("Validation failed", "E001-Validation failed"))
                .WithMessage(new MessageDataModel("Field 'name' is required", "E001-Field name required"),
                    MessageType.Error);

            throw new WebResultException(res);
        }

        [HttpPost("throw-404")]
        public async Task<IActionResult> ThrowNotFound(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();
            res.WithError(new MessageDataModel("Order not found", "E404-OrderNotFound"));

            throw new WebResultException(res, HttpStatusCode.NotFound);
        }

        [HttpPost("throw-with-overrides")]
        public async Task<IActionResult> ThrowWithOverrides(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();
            res.WithError(new MessageDataModel("Conflict on resource", "E409-Conflict"));

            throw new WebResultException(
                res,
                HttpStatusCode.Conflict,
                "Resource already exists",
                "An item with the same key was created earlier.",
                "/api/orders/42");
        }

        [HttpPost("throw-with-extras")]
        public async Task<IActionResult> ThrowWithExtras(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultMultiFailAsync();

            var extras = new Dictionary<string, object>
            {
                ["correlationId"] = "corr-2025-001",
                ["retryable"] = false
            };

            throw new WebResultException(
                (Result)res,
                HttpStatusCode.UnprocessableEntity,
                additionalInformation: extras);
        }
    }
}