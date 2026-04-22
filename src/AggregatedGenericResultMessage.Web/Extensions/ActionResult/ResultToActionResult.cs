// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-07 00:42
//
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 19:46
// ***********************************************************************
//  <copyright file="ResultToActionResult.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
//
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Mappers;

#endregion

namespace RzR.ResultMessage.Web.Extensions.ActionResult
{
    /// <summary>
    ///     Result to Action/Object Result
    ///     Result -> ActionResult
    /// </summary>
    public static partial class ToActionResult
    {
        /// <summary>
        ///     Result to ActionResult.
        /// </summary>
        /// <remarks>
        ///     Status code is resolved by <see cref="ResultStatusCodeMapper.Current" />.
        ///     Defaults: 204 NoContent on success, 400 BadRequest on failure.
        /// </remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this Result result)
            => BuildDefault(result, hasResponseBody: false, successBody: null, failureBody: result.Messages);

        /// <inheritdoc cref="AsActionResult(Result)" />
        public static IActionResult AsIActionResult(this Result result)
            => BuildDefault(result, hasResponseBody: false, successBody: null, failureBody: result.Messages);

        /// <summary>
        ///     Result&lt;T&gt; to ActionResult.
        /// </summary>
        /// <remarks>
        ///     Status code is resolved by <see cref="ResultStatusCodeMapper.Current" />.
        ///     Defaults: 200 OK on success (with Response as body), 400 BadRequest with all
        ///     messages on failure.
        /// </remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this Result<T> result)
            => BuildDefault(result, hasResponseBody: true, successBody: result.Response, failureBody: result.Messages);

        /// <inheritdoc cref="AsActionResult{T}(Result{T})" />
        public static IActionResult AsIActionResult<T>(this Result<T> result)
            => BuildDefault(result, hasResponseBody: true, successBody: result.Response, failureBody: result.Messages);

        /// <inheritdoc cref="AsActionResult(Result)" />
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this IResult result)
            => BuildDefault(result, hasResponseBody: false, successBody: null, failureBody: result.Messages);

        /// <inheritdoc cref="AsActionResult(Result)" />
        public static IActionResult AsIActionResult(this IResult result)
            => BuildDefault(result, hasResponseBody: false, successBody: null, failureBody: result.Messages);

        /// <inheritdoc cref="AsActionResult{T}(Result{T})" />
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this IResult<T> result)
            => BuildDefault(result, hasResponseBody: true, successBody: result.Response, failureBody: result.Messages);

        /// <inheritdoc cref="AsActionResult{T}(Result{T})" />
        public static IActionResult AsIActionResult<T>(this IResult<T> result)
            => BuildDefault(result, hasResponseBody: true, successBody: result.Response, failureBody: result.Messages);

        /// <summary>
        ///     Resolves the response status from <see cref="ResultStatusCodeMapper.Current" /> and
        ///     builds the corresponding <see cref="Microsoft.AspNetCore.Mvc.ActionResult" />.
        ///     A 204 success short-circuits to a body-less <see cref="StatusCodeResult" />.
        /// </summary>
        private static Microsoft.AspNetCore.Mvc.ActionResult BuildDefault(
            IResult result, bool hasResponseBody, object successBody, object failureBody)
        {
            var statusCode = ResultStatusCodeMapper.Current.Map(result, hasResponseBody).ToInt();
            var isSuccess = result.IsSuccess.IsTrue();

            if (isSuccess)
            {
                return statusCode == StatusCodes.Status204NoContent
                    ? (Microsoft.AspNetCore.Mvc.ActionResult)new StatusCodeResult(statusCode)
                    : new ObjectResult(successBody) { StatusCode = statusCode };
            }

            return new ObjectResult(failureBody) { StatusCode = statusCode };
        }
    }
}
