// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-23 12:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 12:25
// ***********************************************************************
//  <copyright file="ResultMessageHttpResults.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#if NET6_0_OR_GREATER

#region U S A G E S

using RzR.ResultMessage.Abstractions;
using System.Collections.Generic;
using System.Net;

using HttpIResult = Microsoft.AspNetCore.Http.IResult;
using IResult = RzR.ResultMessage.Abstractions.IResult;

#endregion

namespace RzR.ResultMessage.Web.Extensions.MinimalApi
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Static facade mirroring <see cref="Microsoft.AspNetCore.Http.Results" />: allows callers
    ///     write <c>ResultMessageResults.From(result)</c> instead of an extension call.
    /// </summary>
    /// =================================================================================================
    public static class ResultMessageHttpResults
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Equivalent to
        ///     <see cref="ResultToHttpResult.ToHttpResult(IResult, HttpStatusCode?, string, string, string, IDictionary{string, object})" />
        ///     .
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">(Optional) The status code.</param>
        /// <param name="message">(Optional) The message.</param>
        /// <param name="detailMessage">(Optional) Message describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionInformation">(Optional) Information describing the addition.</param>
        /// <returns>
        ///     A HttpIResult.
        /// </returns>
        /// =================================================================================================
        public static HttpIResult From(
            IResult result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionInformation = null)
            => result.ToHttpResult(statusCode, message, detailMessage, accessedResourceUri, additionInformation);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Generic overload of
        ///     <see cref="From(IResult, HttpStatusCode?, string, string, string, IDictionary{string, object})" />
        ///     .
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">(Optional) The status code.</param>
        /// <param name="message">(Optional) The message.</param>
        /// <param name="detailMessage">(Optional) Message describing the detail.</param>
        /// <param name="accessedResourceUri">(Optional) URI of the accessed resource.</param>
        /// <param name="additionInformation">(Optional) Information describing the addition.</param>
        /// <returns>
        ///     A HttpIResult.
        /// </returns>
        /// =================================================================================================
        public static HttpIResult From<T>(
            IResult<T> result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionInformation = null)
            => result.ToHttpResult(statusCode, message, detailMessage, accessedResourceUri, additionInformation);
    }
}
#endif