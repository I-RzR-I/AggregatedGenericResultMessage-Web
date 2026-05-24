// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2024-12-26 21:12
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 21:25
// ***********************************************************************
//  <copyright file="ResultProblemDetailsHelper.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Models;
using System.Collections.Generic;
using System.Net;

#endregion

namespace RzR.ResultMessage.Web.Helpers
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     A result problem details helper.
    /// </summary>
    /// =================================================================================================
    internal static class ResultProblemDetailsHelper
    {
        /// <summary>
        ///     Builds object result by delegating to <see cref="ProblemDetailsResultFactory.Current" />
        ///     so global defaults configured via DI are honored. Per-call overrides
        ///     (<paramref name="message" />, <paramref name="detailMessage" />,
        ///     <paramref name="accessedResourceUri" />, <paramref name="additionalInformation" />)
        ///     still win over the factory defaults.
        /// </summary>
        internal static ObjectResult BuildObjectResult(
            IResult result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null)
            => ProblemDetailsResultFactory.Current.Create(new ResultProblemDetailsContext
            {
                Result = result,
                StatusCode = statusCode,
                HasResponseBody = false,
                Response = null,
                Message = message,
                DetailMessage = detailMessage,
                AccessedResourceUri = accessedResourceUri,
                AdditionalInformation = additionalInformation
            });

        /// <summary>
        ///     Generic overload of <see cref="BuildObjectResult(IResult,HttpStatusCode,string,string,string,IDictionary{string,object})" />
        ///     that carries the <c>Result&lt;T&gt;.Response</c> for the success path.
        /// </summary>
        internal static ObjectResult BuildObjectResult<T>(
            IResult<T> result,
            HttpStatusCode statusCode,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionalInformation = null)
            => ProblemDetailsResultFactory.Current.Create(new ResultProblemDetailsContext
            {
                Result = result,
                StatusCode = statusCode,
                HasResponseBody = true,
                Response = result == null ? null : result.Response,
                Message = message,
                DetailMessage = detailMessage,
                AccessedResourceUri = accessedResourceUri,
                AdditionalInformation = additionalInformation
            });
    }
}