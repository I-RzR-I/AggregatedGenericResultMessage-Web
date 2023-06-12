// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApi
//  Author           : RzR
//  Created On       : 2023-06-09 17:06
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-09 17:06
// ***********************************************************************
//  <copyright file="ResultToActionResultController.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using System.Threading;
using System.Threading.Tasks;
using AggregatedGenericResultMessage.Web;
using AggregatedGenericResultMessage.Web.Extensions.ActionResult;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Services;

namespace TestWebApi.Controllers
{
    [ApiController]
    public class ResultToActionResultController : ResultBaseApiController
    {
        private readonly WeatherService _weatherService;

        public ResultToActionResultController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost(nameof(ResultAsActionX1))]
        public async Task<IActionResult> ResultAsActionX1(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultAsync();

            return res.AsActionResult();
        }

        [HttpPost(nameof(ResultFailureAsActionX1))]
        public async Task<IActionResult> ResultFailureAsActionX1(CancellationToken cancellationToken)
        {
            var res = await _weatherService.GetResultFailAsync();

            return res.AsActionResult();
        }
    }
}