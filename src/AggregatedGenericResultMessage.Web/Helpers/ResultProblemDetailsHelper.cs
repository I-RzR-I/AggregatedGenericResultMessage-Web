// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2024-12-25 13:42
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 17:43
// ***********************************************************************
//  <copyright file="ResultProblemDetailsHelper.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.Internal.DataType;
using AggregatedGenericResultMessage.Web.Helpers.Store;
using AggregatedGenericResultMessage.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

#endregion

namespace AggregatedGenericResultMessage.Web.Helpers
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     A result problem details helper.
    /// </summary>
    /// =================================================================================================
    internal static class ResultProblemDetailsHelper
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Builds object result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">(Optional) The message.</param>
        /// <param name="detailMessage">(Optional) Message describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionInformation">(Optional) Information describing the addition.</param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        internal static ObjectResult BuildObjectResult(
            IResult result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionInformation = null)
        {
            var httpCode = statusCode.ToInt();
            if (result.IsSuccess.IsTrue() && httpCode.IsSuccessHttpStatus())
                return new ObjectResult(null) { StatusCode = httpCode };

            var fistMessage = result.GetFirstMessageWithDetails();

            var problemDetails = BuildBaseResultMessageProblemDetails(
                statusCode,
                message.IfIsMissing(fistMessage?.Info),
                detailMessage.IfIsMissing(fistMessage?.ToString()),
                accessedResourceUri
            );

            BuildProblemDetailsExtensionInfo(ref problemDetails, result, additionInformation);

            return new ObjectResult(problemDetails) { StatusCode = httpCode };
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Builds object result.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">(Optional) The message.</param>
        /// <param name="detailMessage">(Optional) Message describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionInformation">(Optional) Information describing the addition.</param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        internal static ObjectResult BuildObjectResult<T>(
            IResult<T> result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionInformation = null)
        {
            var httpCode = statusCode.ToInt();
            if (result.IsSuccess.IsTrue() && httpCode.IsSuccessHttpStatus())
                return new ObjectResult(result.Response) { StatusCode = httpCode };

            var fistMessage = result.GetFirstMessageWithDetails();

            var problemDetails = BuildBaseResultMessageProblemDetails(
                statusCode,
                message.IfIsMissing(fistMessage?.Info),
                detailMessage.IfIsMissing(fistMessage?.ToString()),
                accessedResourceUri
            );

            BuildProblemDetailsExtensionInfo(ref problemDetails, result, additionInformation);

            return new ObjectResult(problemDetails) { StatusCode = httpCode };
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Builds base result message problem details.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="detailMessage">(Optional) Message describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <returns>
        ///     The ResultMessageProblemDetails.
        /// </returns>
        /// =================================================================================================
        private static ResultMessageProblemDetails BuildBaseResultMessageProblemDetails(
            HttpStatusCode statusCode,
            string message,
            string detailMessage = null,
            string accessedResourceUri = null)
        {
            var httpCode = statusCode.ToString();

            var problemDetails = new ResultMessageProblemDetails
            {
                Status = statusCode.ToInt(),
                Title = message,
                Type = RfcTypeHttpCodeDictionary.RfcHttpStatusCodeInfo[httpCode],
                Detail = detailMessage,
                Instance = accessedResourceUri
            };

            return problemDetails;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Builds problem details extension information.
        /// </summary>
        /// <param name="problem">[in,out] The problem.</param>
        /// <param name="result">The result.</param>
        /// <param name="additionInformation">(Optional) Information describing the addition.</param>
        /// <returns>
        ///     The ResultMessageProblemDetails.
        /// </returns>
        /// =================================================================================================
        private static void BuildProblemDetailsExtensionInfo(
            ref ResultMessageProblemDetails problem,
            IResult result,
            IDictionary<string, object> additionInformation = null)
        {
            if (additionInformation.IsNullOrEmptyEnumerable().IsFalse())
                foreach (var addInfo in additionInformation!)
                    problem.Extensions[addInfo.Key] = addInfo.Value;

            problem.Extensions["ResultMessages"] = result.Messages;
        }
    }
}