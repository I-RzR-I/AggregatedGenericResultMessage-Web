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

using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Extensions.Internal.Result;
using RzR.ResultMessage.Web.Helpers;
using System.Net;

// ReSharper disable RedundantCast

#endregion

namespace RzR.ResultMessage.Web.Extensions.ActionResult
{
    /// <summary>
    ///     Result to Action/Object Result
    ///     Result to ActionResult by HTTP status code
    /// </summary>
    public static partial class ToActionResult
    {
        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="Result" /></returns>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this Result result, HttpStatusCode statusCode)
            => BuildActionResult((IResult)result, statusCode, result.Messages);

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="Result" /></returns>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult(this Result result, HttpStatusCode statusCode)
            => BuildActionResult((IResult)result, statusCode, result.Messages);

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="Result{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this Result<T> result, HttpStatusCode statusCode)
            => BuildActionResult(result, statusCode, result.Messages);

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="Result{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult<T>(this Result<T> result, HttpStatusCode statusCode)
            => BuildActionResult(result, statusCode, result.Messages);

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="IResult" /></returns>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this IResult result, HttpStatusCode statusCode)
            => BuildActionResult(result, statusCode, result.Messages);

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="IResult" /></returns>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult(this IResult result, HttpStatusCode statusCode)
            => BuildActionResult(result, statusCode, result.Messages);

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="ActionResult" /> from <see cref="IResult{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
            => BuildActionResult(result, statusCode, result.Messages);

        /// <summary>
        ///     Result to IActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Return <see cref="IActionResult" /> from <see cref="IResult{T}" /></returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
            => BuildActionResult(result, statusCode, result.Messages);

        /// <summary>
        ///     Builds an <see cref="Microsoft.AspNetCore.Mvc.ActionResult"/> from a non-generic result
        ///     and an HTTP status code.
        /// </summary>
        /// <param name="result">Common result.</param>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="failurePayload">Payload to return on failure or invalid status code.</param>
        /// <returns>
        ///     <see cref="StatusCodeResult"/> on success, or an <see cref="ObjectResult"/> /
        ///     <see cref="BadRequestObjectResult"/> containing the failure payload otherwise.
        /// </returns>
        private static Microsoft.AspNetCore.Mvc.ActionResult BuildActionResult(
            IResult result, HttpStatusCode statusCode, object failurePayload)
        {
            var httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);

            return statusCodeCheck.IsNoSuccess()
                ? new BadRequestObjectResult(failurePayload) { StatusCode = httpStatusCode }
                : result.IsWithSuccess(statusCodeCheck)
                    ? (Microsoft.AspNetCore.Mvc.ActionResult)new StatusCodeResult(httpStatusCode)
                    : new ObjectResult(failurePayload) { StatusCode = httpStatusCode };
        }

        /// <summary>
        ///     Builds an <see cref="Microsoft.AspNetCore.Mvc.ActionResult"/> from a generic result
        ///     and an HTTP status code.
        /// </summary>
        /// <typeparam name="T">Result response type.</typeparam>
        /// <param name="result">Common result.</param>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="failurePayload">Payload to return on failure or invalid status code.</param>
        /// <returns>
        ///     <see cref="ObjectResult"/> wrapping <see cref="IResult{T}.Response"/> on success, or an
        ///     <see cref="ObjectResult"/> / <see cref="BadRequestObjectResult"/> containing the failure payload otherwise.
        /// </returns>
        private static Microsoft.AspNetCore.Mvc.ActionResult BuildActionResult<T>(
            IResult<T> result, HttpStatusCode statusCode, object failurePayload)
        {
            var httpStatusCode = statusCode.ToInt();
            var statusCodeCheck = CheckResultStatusHelper.CheckStatusCode(statusCode);

            return statusCodeCheck.IsNoSuccess()
                ? new BadRequestObjectResult(failurePayload) { StatusCode = httpStatusCode }
                : result.IsWithSuccess(statusCodeCheck)
                    ? new ObjectResult(result.Response) { StatusCode = httpStatusCode }
                    : new ObjectResult(failurePayload) { StatusCode = httpStatusCode };
        }
    }
}