// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2026-04-22 19:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:01
// ***********************************************************************
//  <copyright file="ResultMultiMessagesController.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Web;
using RzR.ResultMessage.Web.Extensions.ActionResult;
using TestWebApi.Services;

#endregion

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResultMultiMessagesController : ResultBaseApiController
    {
        private readonly WeatherService _weatherService;

        public ResultMultiMessagesController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost(nameof(MultiFailureAsAction))]
        public async Task<IActionResult> MultiFailureAsAction(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultMultiFailAsync();

            return res.AsActionResult();
        }

        [HttpPost(nameof(MultiFailureGenericAsAction))]
        public async Task<IActionResult> MultiFailureGenericAsAction(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionMultiFailAsync();

            return res.AsActionResult();
        }

        [HttpPost(nameof(MultiFailureGenericAsAction422))]
        public async Task<IActionResult> MultiFailureGenericAsAction422(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionMultiFailAsync();

            return res.AsActionResult(HttpStatusCode.UnprocessableEntity);
        }

        [HttpPost(nameof(EnvelopeSuccess))]
        public async Task<IActionResult> EnvelopeSuccess(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultAsync();

            return res.AsEnvelopeIActionResult();
        }

        [HttpPost(nameof(EnvelopeFailureMulti))]
        public async Task<IActionResult> EnvelopeFailureMulti(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultMultiFailAsync();

            return res.AsEnvelopeIActionResult();
        }

        [HttpPost(nameof(EnvelopeGenericFailure422))]
        public async Task<IActionResult> EnvelopeGenericFailure422(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionMultiFailAsync();

            return res.AsEnvelopeIActionResult(HttpStatusCode.UnprocessableEntity);
        }

        [HttpPost(nameof(EnvelopeGenericSuccess201))]
        public async Task<IActionResult> EnvelopeGenericSuccess201(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetCollectionAsync();

            return res.AsEnvelopeIActionResult(HttpStatusCode.Created);
        }
    }
}