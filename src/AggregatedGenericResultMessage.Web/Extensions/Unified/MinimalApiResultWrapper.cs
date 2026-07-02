// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author            : RzR
//  Created           : 02-07-2026 19:07
// 
//  Last Modified By : RzR
//  Last Modified On : 02-07-2026 20:16
//  ***********************************************************************
//  <copyright file="MinimalApiResultWrapper.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

#if NET6_0_OR_GREATER

#region U S I N G

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using HttpIResult = Microsoft.AspNetCore.Http.IResult;

#endregion

namespace RzR.ResultMessage.Web.Extensions.Unified
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Single source of truth for the terminal Minimal-API wrap step: turning a resolved
    ///     response body/status pair into a <see cref="HttpIResult" />. Reused by
    ///     <see cref="RzR.ResultMessage.Web.Extensions.MinimalApi.ResultToHttpResult" /> and by
    ///     <see cref="ResultMessageResponse" /> so both entry points render byte-identical responses.
    /// </summary>
    /// =================================================================================================
    internal static class MinimalApiResultWrapper
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Wraps an already-resolved response body and HTTP status code into a Minimal-API
        ///     <see cref="HttpIResult" />. A <see langword="null" /> body short-circuits to
        ///     <see cref="Results.StatusCode(int)" />; a <see cref="ProblemDetails" /> body is written
        ///     with the <c>application/problem+json</c> content type so spec-aware clients see it correctly.
        /// </summary>
        /// <param name="body">The resolved response body, or <see langword="null" /> for an empty body.</param>
        /// <param name="resolvedStatus">The resolved HTTP status code to write on the response.</param>
        /// <returns>
        ///     A HttpIResult.
        /// </returns>
        /// =================================================================================================
        internal static HttpIResult Wrap(object body, int resolvedStatus)
        {
            if (body.IsNull())
                return Results.StatusCode(resolvedStatus);

            var contentType = body is ProblemDetails ? "application/problem+json" : null;

            return Results.Json(body, statusCode: resolvedStatus, contentType: contentType);
        }
    }
}
#endif