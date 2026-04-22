// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:21
// ***********************************************************************
//  <copyright file="IResultStatusCodeMapper.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Abstractions;
using System.Net;

#endregion

namespace RzR.ResultMessage.Web.Abstractions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Strategy that maps an <see cref="IResult" /> envelope to an <see cref="HttpStatusCode" />
    ///     for transport. Implement and register via
    ///     <c>services.AddResultMessageWeb&lt;TMapper&gt;()</c> to override the defaults
    ///     (200 / 204 on success, 400 on failure) and unlock semantics like 404 / 409 / 422 without
    ///     touching call sites.
    /// </summary>
    /// =================================================================================================
    public interface IResultStatusCodeMapper
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the HTTP status code for the given <paramref name="result" />.
        /// </summary>
        /// <param name="result">The result envelope. Never <see langword="null" />.</param>
        /// <param name="hasResponseBody">
        ///     <see langword="true" /> when the caller intends to write a response body (i.e. <c>
        ///     Result&lt;T&gt;</c> with a non-null <c>Response</c>); allows the mapper to pick 200 vs
        ///     204 on success.
        /// </param>
        /// <returns>
        ///     The HTTP status code to apply to the outgoing response.
        /// </returns>
        /// =================================================================================================
        HttpStatusCode Map(IResult result, bool hasResponseBody);
    }
}