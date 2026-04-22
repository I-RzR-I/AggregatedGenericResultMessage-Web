// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 19:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 19:26
// ***********************************************************************
//  <copyright file="ResultToActionResultEnvelope.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
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
using RzR.ResultMessage.Web.Extensions.Internal.Result;
using RzR.ResultMessage.Web.Helpers;
using System.Net;

// ReSharper disable RedundantCast

#endregion

namespace RzR.ResultMessage.Web.Extensions.ActionResult
{
    /// -------------------------------------------------------------------------------------------------
    /// <content>
    ///     Result to Action/Object Result. Opt-in API that serializes the full <see cref="IResult" />
    ///     /<see cref="IResult{T}" /> envelope.
    /// </content>
    /// =================================================================================================
    public static partial class ToActionResult
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the full <see cref="IResult" /> envelope as the response body. HTTP status
        ///     defaults to 200 on success, 400 on failure.
        /// </summary>
        /// <param name="result">Common result.</param>
        /// <returns>
        ///     An <see cref="Microsoft.AspNetCore.Mvc.ActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static Microsoft.AspNetCore.Mvc.ActionResult AsEnvelopeActionResult(this IResult result)
            => BuildEnvelopeActionResult(result,
                result.IsSuccess.IsTrue() ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the full <see cref="IResult" /> envelope as the response body with a caller-
        ///     supplied HTTP status code.
        /// </summary>
        /// <param name="result">Common result.</param>
        /// <param name="statusCode">HTTP status code applied to the response.</param>
        /// <returns>
        ///     An <see cref="Microsoft.AspNetCore.Mvc.ActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static Microsoft.AspNetCore.Mvc.ActionResult AsEnvelopeActionResult(this IResult result, HttpStatusCode statusCode)
            => BuildEnvelopeActionResult(result, statusCode);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the full <see cref="IResult{T}" /> envelope as the response body. HTTP status
        ///     defaults to 200 on success, 400 on failure.
        /// </summary>
        /// <typeparam name="T">Result response type.</typeparam>
        /// <param name="result">Common result.</param>
        /// <returns>
        ///     An <see cref="Microsoft.AspNetCore.Mvc.ActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static Microsoft.AspNetCore.Mvc.ActionResult AsEnvelopeActionResult<T>(this IResult<T> result)
            => BuildEnvelopeActionResult(result,
                result.IsSuccess.IsTrue() ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the full <see cref="IResult{T}" /> envelope as the response body with a caller-
        ///     supplied HTTP status code.
        /// </summary>
        /// <typeparam name="T">Result response type.</typeparam>
        /// <param name="result">Common result.</param>
        /// <param name="statusCode">HTTP status code applied to the response.</param>
        /// <returns>
        ///     An <see cref="Microsoft.AspNetCore.Mvc.ActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static Microsoft.AspNetCore.Mvc.ActionResult AsEnvelopeActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
            => BuildEnvelopeActionResult(result, statusCode);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     <see cref="IActionResult" /> alias of <see cref="AsEnvelopeActionResult(IResult)" />.
        /// </summary>
        /// <param name="result">Common result.</param>
        /// <returns>
        ///     An <see cref="IActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static IActionResult AsEnvelopeIActionResult(this IResult result)
            => AsEnvelopeActionResult(result);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     <see cref="IActionResult" /> alias of <see cref="AsEnvelopeActionResult(IResult, HttpStatusCode)" />
        ///     .
        /// </summary>
        /// <param name="result">Common result.</param>
        /// <param name="statusCode">HTTP status code applied to the response.</param>
        /// <returns>
        ///     An <see cref="IActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static IActionResult AsEnvelopeIActionResult(this IResult result, HttpStatusCode statusCode)
            => AsEnvelopeActionResult(result, statusCode);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     <see cref="IActionResult" /> alias of <see cref="AsEnvelopeActionResult{T}(IResult{T})" />
        ///     .
        /// </summary>
        /// <typeparam name="T">Result response type.</typeparam>
        /// <param name="result">Common result.</param>
        /// <returns>
        ///     An <see cref="IActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static IActionResult AsEnvelopeIActionResult<T>(this IResult<T> result)
            => AsEnvelopeActionResult(result);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     <see cref="IActionResult" /> alias of <see cref="AsEnvelopeActionResult{T}(IResult{T}, HttpStatusCode)" />
        ///     .
        /// </summary>
        /// <typeparam name="T">Result response type.</typeparam>
        /// <param name="result">Common result.</param>
        /// <param name="statusCode">HTTP status code applied to the response.</param>
        /// <returns>
        ///     An <see cref="IActionResult" /> wrapping the full envelope.
        /// </returns>
        /// =================================================================================================
        public static IActionResult AsEnvelopeIActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
            => AsEnvelopeActionResult(result, statusCode);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Builds an <see cref="ObjectResult" /> that wraps the full <paramref name="result" />
        ///     envelope using the supplied <paramref name="statusCode" />. If the supplied status is
        ///     invalid the response falls back to <see cref="StatusCodes.Status400BadRequest" />.
        /// 
        /// </summary>
        /// <param name="result">Result envelope to serialize.</param>
        /// <param name="statusCode">HTTP status code applied to the response.</param>
        /// <returns>
        ///     An <see cref="ObjectResult" /> containing the envelope.
        /// </returns>
        /// =================================================================================================
        private static Microsoft.AspNetCore.Mvc.ActionResult BuildEnvelopeActionResult(
            object result, HttpStatusCode statusCode)
        {
            var httpStatusCode = statusCode.ToInt();
            var isValidHttpCode = httpStatusCode >= 100 && httpStatusCode <= 599;

            return isValidHttpCode
                ? new ObjectResult(result) { StatusCode = httpStatusCode }
                : new ObjectResult(result) { StatusCode = StatusCodes.Status400BadRequest };
        }
    }
}