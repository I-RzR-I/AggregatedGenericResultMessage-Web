// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2026-04-22 21:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 21:54
// ***********************************************************************
//  <copyright file="BrandedProblemDetailsController.cs" company="RzR SOFT & TECH">
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
using RzR.ResultMessage.Web.Extensions.ProblemDetail;
using TestWebApi.Services;

#endregion

namespace TestWebApi.Controllers
{
    /// <summary>
    ///     Demonstrates the configurable <c>IProblemDetailsResultFactory</c>:
    ///     <list type="bullet">
    ///         <item>Default branded output (factory supplies type/instance/extensions).</item>
    ///         <item>Per-call override of <c>title</c>/<c>detail</c>/<c>instance</c>.</item>
    ///         <item>Per-call additional extensions merged with the factory's defaults.</item>
    ///         <item>Both 4xx and 5xx flows.</item>
    ///     </list>
    /// </summary>
    [ApiController]
    [Route("api/branded-problem-details")]
    public class BrandedProblemDetailsController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public BrandedProblemDetailsController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        ///     Success path: factory short-circuits to <c>200 OK</c> with the response body.
        /// </summary>
        [HttpGet("success")]
        public async Task<IActionResult> Success(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionAsync();

            return res.AsProblemDetails(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Failure path with no per-call overrides — factory supplies
        ///     <c>type</c>, <c>instance</c>, <c>traceId</c>, and <c>service</c> globally.
        /// </summary>
        [HttpPost("failure-defaults")]
        public async Task<IActionResult> FailureDefaults(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();

            res.WithError(new MessageDataModel("Validation failed", "E001-Validation failed"))
                .WithMessage(new MessageDataModel("Field 'name' is required", "E001-Field name required"),
                    MessageType.Error);

            return res.AsProblemDetails(HttpStatusCode.BadRequest);
        }

        /// <summary>
        ///     Failure path with per-call <c>title</c>, <c>detail</c>, <c>instance</c>
        ///     overrides — these win over factory defaults.
        /// </summary>
        [HttpPost("failure-per-call-overrides")]
        public async Task<IActionResult> FailurePerCallOverrides(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();
            res.WithError(new MessageDataModel("Conflict on resource", "E409-Conflict"));

            return res.AsProblemDetails(
                HttpStatusCode.Conflict,
                "Resource already exists",
                "An item with the same key was created earlier.",
                $"/api/orders/{42}");
        }

        /// <summary>
        ///     Failure path with caller-supplied additional extensions merged with the
        ///     factory's defaults (<c>traceId</c>, <c>service</c>, <c>ResultMessages</c>).
        /// </summary>
        [HttpPost("failure-extra-extensions")]
        public async Task<IActionResult> FailureExtraExtensions(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultMultiFailAsync();

            var extras = new Dictionary<string, object>
            {
                ["correlationId"] = "corr-2025-001",
                ["retryable"] = false
            };

            return ((Result)res).AsProblemDetails(
                HttpStatusCode.UnprocessableEntity,
                additionalInformation: extras);
        }

        /// <summary>
        ///     5xx flow: factory still applies branding and extensions to server errors.
        /// </summary>
        [HttpPost("failure-internal")]
        public async Task<IActionResult> FailureInternal(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();
            res.WithError(new MessageDataModel("Upstream service unavailable", "E503-Upstream"));

            return res.AsProblemDetails(
                HttpStatusCode.InternalServerError,
                detailMessage: "An unexpected error occurred while processing the request.");
        }

        /// <summary>
        ///     Generic <c>IResult&lt;T&gt;</c> failure path — factory still produces the
        ///     branded ProblemDetails (response body is dropped on failure).
        /// </summary>
        [HttpPost("failure-generic")]
        public async Task<IActionResult> FailureGeneric(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionMultiFailAsync();

            return res.AsProblemDetails(HttpStatusCode.BadRequest);
        }
    }
}