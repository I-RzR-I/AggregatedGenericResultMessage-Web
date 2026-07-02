// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2024-12-24 13:56
//
//  Last Modified By : RzR
//  Last Modified On : 2024-12-26 18:30
// ***********************************************************************
//  <copyright file="ResultToProblemDetails.cs" company="RzR SOFT & TECH">
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
using RzR.ResultMessage.Web.Helpers;
using System.Collections.Generic;
using System.Net;

using IResult = RzR.ResultMessage.Abstractions.IResult;

#endregion

namespace RzR.ResultMessage.Web.Extensions.ProblemDetail
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     `Result` extension methods that are converted to the `ProblemDetails` model with customization possibilities.
    /// </summary>
    /// =================================================================================================
    public static class ResultToProblemDetails
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails<T>(
            this Result<T> result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null)
            => AsProblemDetails(result, (HttpStatusCode?)statusCode, message, detailMessage, accessedResourceUri, additionalInformation);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details.
        /// </summary>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails(
            this Result result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null)
            => AsProblemDetails(result, (HttpStatusCode?)statusCode, message, detailMessage, accessedResourceUri, additionalInformation);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails<T>(
            this IResult<T> result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null)
            => AsProblemDetails(result, (HttpStatusCode?)statusCode, message, detailMessage, accessedResourceUri, additionalInformation);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details.
        /// </summary>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionalInformation">(Optional) Additional extension members to merge into the ProblemDetails response.</param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails(
            this IResult result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null)
            => AsProblemDetails(result, (HttpStatusCode?)statusCode, message, detailMessage, accessedResourceUri, additionalInformation);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details, resolving the
        ///     status code and enabling <c>traceId</c> correlation when not supplied explicitly.
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
        ///     <see cref="HttpContext" /> from the controller when correlation is desired.
        /// </param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails<T>(
            this Result<T> result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
            => ResultProblemDetailsHelper.BuildObjectResult(result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details, resolving the
        ///     status code and enabling <c>traceId</c> correlation when not supplied explicitly.
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
        ///     <see cref="HttpContext" /> from the controller when correlation is desired.
        /// </param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails(
            this Result result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
            => ResultProblemDetailsHelper.BuildObjectResult((IResult)result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details, resolving the
        ///     status code and enabling <c>traceId</c> correlation when not supplied explicitly.
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
        ///     <see cref="HttpContext" /> from the controller when correlation is desired.
        /// </param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails<T>(
            this IResult<T> result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
            => ResultProblemDetailsHelper.BuildObjectResult(result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to problem details, resolving the
        ///     status code and enabling <c>traceId</c> correlation when not supplied explicitly.
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
        ///     <see cref="HttpContext" /> from the controller when correlation is desired.
        /// </param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        public static ObjectResult AsProblemDetails(
            this IResult result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null,
            HttpContext httpContext = null)
            => ResultProblemDetailsHelper.BuildObjectResult(result, statusCode, message, detailMessage, accessedResourceUri, additionalInformation, httpContext);
    }
}