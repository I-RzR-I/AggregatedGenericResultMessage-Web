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

using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.ProblemDetail
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
        /// <param name="additionInformation">(Optional) Information describing the addition `ProblemDetails` objects.</param>
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
            IDictionary<string, object> additionInformation = null)
            => ResultProblemDetailsHelper.BuildObjectResult(result, statusCode, message, detailMessage, accessedResourceUri, additionInformation);

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
        /// <param name="additionInformation">(Optional) Information describing the addition `ProblemDetails` objects.</param>
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
            IDictionary<string, object> additionInformation = null)
            => ResultProblemDetailsHelper.BuildObjectResult(result, statusCode, message, detailMessage, accessedResourceUri, additionInformation);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to to problem details.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionInformation">(Optional) Information describing the addition `ProblemDetails` objects.</param>
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
            IDictionary<string, object> additionInformation = null)
            => ResultProblemDetailsHelper.BuildObjectResult(result, statusCode, message, detailMessage, accessedResourceUri, additionInformation);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IResult extension method that converts this object to to problem details.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result to act on.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">(Optional) The custom `ProblemDetails` message.</param>
        /// <param name="detailMessage">(Optional) The custom message `ProblemDetails` describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionInformation">(Optional) Information describing the addition `ProblemDetails` objects.</param>
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
            IDictionary<string, object> additionInformation = null)
            => ResultProblemDetailsHelper.BuildObjectResult(result, statusCode, message, detailMessage, accessedResourceUri, additionInformation);
    }
}