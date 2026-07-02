// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-23 08:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 12:22
// ***********************************************************************
//  <copyright file="ResultToHttpResult.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#if NET6_0_OR_GREATER

#region U S A G E S

using Microsoft.AspNetCore.Http;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Extensions.Unified;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Models;
using System.Collections.Generic;
using System.Net;

using HttpIResult = Microsoft.AspNetCore.Http.IResult;
using IResult = RzR.ResultMessage.Abstractions.IResult;

#endregion

namespace RzR.ResultMessage.Web.Extensions.MinimalApi
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Minimal-API adapters that convert a library <see cref="IResult" /> /
    ///     <see cref="IResult{T}" /> envelope into a
    ///     <see cref="Microsoft.AspNetCore.Http.IResult" /> using the configured
    ///     <see cref="ProblemDetailsResultFactory.Current" /> for failure responses and
    ///     <see cref="ResultStatusCodeMapper.Current" /> for status-code resolution when
    ///     none is provided.
    /// </summary>
    /// =================================================================================================
    public static class ResultToHttpResult
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Converts a non-generic <see cref="IResult" /> into a Minimal-API
        ///     <see cref="Microsoft.AspNetCore.Http.IResult" />. Success returns
        ///     <c>NoContent</c> (204) by default; failure routes through the configured
        ///     ProblemDetails factory.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">
        ///     (Optional) Explicit status code. When <c>null</c>, the
        ///     <see cref="ResultStatusCodeMapper.Current" /> resolves it.
        /// </param>
        /// <param name="message">(Optional) Per-call ProblemDetails title override.</param>
        /// <param name="detailMessage">(Optional) Per-call ProblemDetails detail override.</param>
        /// <param name="accessedResourceUri">
        ///     (Optional) Per-call ProblemDetails instance override.
        /// </param>
        /// <param name="additionalInformation">
        ///     (Optional) Per-call ProblemDetails extension members.
        /// </param>
        /// <param name="httpContext">
        ///     (Optional) Ambient <see cref="HttpContext" /> used by the configured
        ///     <see cref="ProblemDetailsResultFactory.Current" /> for correlation (e.g. autopopulating
        ///     <c>traceId</c>). Pass the request's <see cref="HttpContext" /> from the Minimal-API
        ///     handler when correlation is desired.
        /// </param>
        /// <returns>
        ///     The given data converted to a HttpIResult.
        /// </returns>
        /// =================================================================================================
        public static HttpIResult ToHttpResult(
            this IResult result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
        {
            var resolvedStatus = statusCode ?? ResultStatusCodeMapper.Current.Map(result, false);

            return BuildHttpResult(
                result,
                resolvedStatus,
                false,
                null,
                message,
                detailMessage,
                accessedResourceUri,
                additionalInformation,
                httpContext);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Generic overload that carries <c>IResult&lt;T&gt;.Response</c> on the success path.
        ///     Success returns <c>200 OK</c> with the response body by default.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">(Optional) The status code.</param>
        /// <param name="message">(Optional) The message.</param>
        /// <param name="detailMessage">(Optional) Message describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <param name="httpContext">
        ///     (Optional) Ambient <see cref="HttpContext" /> for correlation. See the non-generic
        ///     overload for details.
        /// </param>
        /// <returns>
        ///     The given data converted to a HttpIResult.
        /// </returns>
        /// =================================================================================================
        public static HttpIResult ToHttpResult<T>(
            this IResult<T> result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
        {
            var resolvedStatus = statusCode ?? ResultStatusCodeMapper.Current.Map(result, true);

            return BuildHttpResult(
                result,
                resolvedStatus,
                true,
                result == null ? null : result.Response,
                message,
                detailMessage,
                accessedResourceUri,
                additionalInformation,
                httpContext);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Shared builder. Delegates ProblemDetails shape to the configured
        ///     <see cref="ProblemDetailsResultFactory.Current" /> so Minimal-API and MVC paths render
        ///     the same body.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="hasResponseBody">True if it has response body, false if not.</param>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        /// <param name="detailMessage">Message describing the detail.</param>
        /// <param name="accessedResourceUri">URI of the accessed resource.</param>
        /// <param name="additionalInformation">Additional extension members to merge into the ProblemDetails response.</param>
        /// <param name="httpContext">Ambient HttpContext for correlation (can be null).</param>
        /// <returns>
        ///     A HttpIResult.
        /// </returns>
        /// =================================================================================================
        private static HttpIResult BuildHttpResult(
            IResult result,
            HttpStatusCode statusCode,
            bool hasResponseBody,
            object response,
            string message,
            string detailMessage,
            string accessedResourceUri,
            IDictionary<string, object> additionalInformation,
            HttpContext httpContext)
        {
            var objectResult = ProblemDetailsResultFactory.Current.Create(new ResultProblemDetailsContext
            {
                Result = result,
                StatusCode = statusCode,
                HasResponseBody = hasResponseBody,
                Response = response,
                Message = message,
                DetailMessage = detailMessage,
                AccessedResourceUri = accessedResourceUri,
                AdditionalInformation = additionalInformation,
                HttpContext = httpContext
            });

            var resolvedStatus = objectResult.StatusCode ?? statusCode.ToInt();

            return MinimalApiResultWrapper.Wrap(objectResult.Value, resolvedStatus);
        }
    }
}
#endif