// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author            : RzR
//  Created           : 02-07-2026 19:07
// 
//  Last Modified By : RzR
//  Last Modified On : 02-07-2026 20:16
//  ***********************************************************************
//  <copyright file="ToProblemResponseExtensions.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

#region U S I N G

using Microsoft.AspNetCore.Http;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Helpers;
using System.Collections.Generic;
using System.Net;
using IResult = RzR.ResultMessage.Abstractions.IResult;

#endregion

namespace RzR.ResultMessage.Web.Extensions.Unified
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     `Result` extension methods that render directly to a single unified framework response
    ///     (<see cref="ResultMessageResponse" />) usable from both MVC controllers and Minimal-API
    ///     handlers, resolving the status code and enabling <c>traceId</c> correlation when not
    ///     supplied explicitly.
    /// </summary>
    /// =================================================================================================
    public static class ToProblemResponseExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A Result&lt;T&gt; extension method that converts this object to a unified problem response.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">
        ///     (Optional) The HTTP status code. When <see langword="null" />, the configured
        ///     <see cref="RzR.ResultMessage.Web.Mappers.ResultStatusCodeMapper.Current" /> resolves it.
        /// </param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <param name="httpContext">
        ///     (Optional) Ambient <see cref="HttpContext" /> used by the configured
        ///     <see cref="RzR.ResultMessage.Web.Factories.ProblemDetailsResultFactory.Current" /> for
        ///     correlation (e.g. autopopulating <c>traceId</c>). Pass the request's
        ///     <see cref="HttpContext" /> when correlation is desired.
        /// </param>
        /// <returns>
        ///     A ResultMessageResponse usable as either an MVC <see cref="Microsoft.AspNetCore.Mvc.IActionResult" />
        ///     or (net6+) a Minimal-API <c>Microsoft.AspNetCore.Http.IResult</c>.
        /// </returns>
        /// =================================================================================================
        public static ResultMessageResponse ToProblemResponse<T>(
            this Result<T> result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
            => new(ResultProblemDetailsHelper.BuildObjectResult(
                result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext));

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A Result extension method that converts this object to a unified problem response.
        /// </summary>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">
        ///     (Optional) The HTTP status code. When <see langword="null" />, the configured
        ///     <see cref="RzR.ResultMessage.Web.Mappers.ResultStatusCodeMapper.Current" /> resolves it.
        /// </param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <param name="httpContext">
        ///     (Optional) Ambient <see cref="HttpContext" /> used by the configured
        ///     <see cref="RzR.ResultMessage.Web.Factories.ProblemDetailsResultFactory.Current" /> for
        ///     correlation (e.g. autopopulating <c>traceId</c>). Pass the request's
        ///     <see cref="HttpContext" /> when correlation is desired.
        /// </param>
        /// <returns>
        ///     A ResultMessageResponse usable as either an MVC <see cref="Microsoft.AspNetCore.Mvc.IActionResult" />
        ///     or (net6+) a Minimal-API <c>Microsoft.AspNetCore.Http.IResult</c>.
        /// </returns>
        /// =================================================================================================
        public static ResultMessageResponse ToProblemResponse(
                this Result result,
                HttpStatusCode? statusCode = null,
                string message = null,
                string detailMessage = null,
                string accessedResourceUri = null,
                IDictionary<string, object> additionalInformation = null,
                HttpContext httpContext = null)
            => new(ResultProblemDetailsHelper.BuildObjectResult(
                (IResult)result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext));

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult&lt;T&gt; extension method that converts this object to a unified problem response.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">
        ///     (Optional) The HTTP status code. When <see langword="null" />, the configured
        ///     <see cref="RzR.ResultMessage.Web.Mappers.ResultStatusCodeMapper.Current" /> resolves it.
        /// </param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <param name="httpContext">
        ///     (Optional) Ambient <see cref="HttpContext" /> used by the configured
        ///     <see cref="RzR.ResultMessage.Web.Factories.ProblemDetailsResultFactory.Current" /> for
        ///     correlation (e.g. autopopulating <c>traceId</c>). Pass the request's
        ///     <see cref="HttpContext" /> when correlation is desired.
        /// </param>
        /// <returns>
        ///     A ResultMessageResponse usable as either an MVC <see cref="Microsoft.AspNetCore.Mvc.IActionResult" />
        ///     or (net6+) a Minimal-API <c>Microsoft.AspNetCore.Http.IResult</c>.
        /// </returns>
        /// =================================================================================================
        public static ResultMessageResponse ToProblemResponse<T>(
            this IResult<T> result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
            => new(ResultProblemDetailsHelper.BuildObjectResult(
                result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext));

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to a unified problem response.
        /// </summary>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">
        ///     (Optional) The HTTP status code. When <see langword="null" />, the configured
        ///     <see cref="RzR.ResultMessage.Web.Mappers.ResultStatusCodeMapper.Current" /> resolves it.
        /// </param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <param name="httpContext">
        ///     (Optional) Ambient <see cref="HttpContext" /> used by the configured
        ///     <see cref="RzR.ResultMessage.Web.Factories.ProblemDetailsResultFactory.Current" /> for
        ///     correlation (e.g. autopopulating <c>traceId</c>). Pass the request's
        ///     <see cref="HttpContext" /> when correlation is desired.
        /// </param>
        /// <returns>
        ///     A ResultMessageResponse usable as either an MVC <see cref="Microsoft.AspNetCore.Mvc.IActionResult" />
        ///     or (net6+) a Minimal-API <c>Microsoft.AspNetCore.Http.IResult</c>.
        /// </returns>
        /// =================================================================================================
        public static ResultMessageResponse ToProblemResponse(
            this IResult result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
            => new(ResultProblemDetailsHelper.BuildObjectResult(
                result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext));
    }
}