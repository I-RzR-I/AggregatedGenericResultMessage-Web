// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-07 00:32
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-08 17:46
// ***********************************************************************
//  <copyright file="ResultToActionResultByCode.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Linq;
using System.Net;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.Internal;
using AggregatedGenericResultMessage.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.ActionResult
{
    /// <summary>
    ///     Result to Action/Object Result
    ///     Result to ActionResult by HTTP status code
    /// </summary>
    public static partial class ToActionResult
    {
        private static int _httpStatusCode = 200;

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="Result" /></returns>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this Result result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);
           
            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? (Microsoft.AspNetCore.Mvc.ActionResult)new StatusCodeResult(_httpStatusCode)
                    : new ObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode };
        }

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="Result" /></returns>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult(this Result result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);

            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? (Microsoft.AspNetCore.Mvc.ActionResult)new StatusCodeResult(_httpStatusCode)
                    : new ObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode };
        }

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="Result{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this Result<T> result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);
            
            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? new ObjectResult(result.Response) { StatusCode = _httpStatusCode }
                    : new ObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode };
        }

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="Result{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult<T>(this Result<T> result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);
            
            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? new ObjectResult(result.Response) { StatusCode = _httpStatusCode }
                    : new ObjectResult(result.GetFirstMessage()) { StatusCode = _httpStatusCode };
        }

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="IResult" /></returns>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this IResult result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);
            
            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? (Microsoft.AspNetCore.Mvc.ActionResult)new StatusCodeResult(_httpStatusCode)
                    : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode };
        }

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="IResult" /></returns>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult(this IResult result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);
           
            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? (IActionResult)new StatusCodeResult(_httpStatusCode)
                    : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode };
        }

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="IResult{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);
           
            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? new ObjectResult(result.Response) { StatusCode = _httpStatusCode }
                    : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode };
        }

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="IResult{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);
            
            return !statusCodeCheck.IsSuccess || statusCodeCheck.Response.IsError
                ? new BadRequestObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode }
                : result.IsSuccess && statusCodeCheck.Response.IsSuccess
                    ? new ObjectResult(result.Response) { StatusCode = _httpStatusCode }
                    : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = _httpStatusCode };
        }
    }
}