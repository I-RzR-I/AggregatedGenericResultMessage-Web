// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:55
// ***********************************************************************
//  <copyright file="ProblemDetailsContext.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Abstractions;
using System.Collections.Generic;
using System.Net;

using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

#endregion

namespace RzR.ResultMessage.Web.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Per-call inputs handed to <see cref="IProblemDetailsResultFactory" />. Per-call values (<see cref="Message" />
    ///     , <see cref="DetailMessage" />,
    ///     <see cref="AccessedResourceUri" />, <see cref="AdditionInformation" />) are intended to
    ///     win over factory-supplied defaults when present.
    /// </summary>
    /// =================================================================================================
    public sealed class ResultProblemDetailsContext
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     The result envelope being serialized.
        /// </summary>
        /// <value>
        ///     The result.
        /// </value>
        /// =================================================================================================
        public IResult Result { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     HTTP status code that will be applied to the response.
        /// </summary>
        /// <value>
        ///     The status code.
        /// </value>
        /// =================================================================================================
        public HttpStatusCode StatusCode { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     <see langword="true" /> for <c>Result&lt;T&gt;</c> where the caller intends to write
        ///     <see cref="Response" /> as the response body on success.
        /// </summary>
        /// <value>
        ///     True if this object has response body, false if not.
        /// </value>
        /// =================================================================================================
        public bool HasResponseBody { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     The <c>Result&lt;T&gt;.Response</c> when <see cref="HasResponseBody" /> is set.
        /// </summary>
        /// <value>
        ///     The response.
        /// </value>
        /// =================================================================================================
        public object Response { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Optional caller-supplied title (overrides factory default).
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>
        /// =================================================================================================
        public string Message { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Optional caller-supplied detail (overrides factory default).
        /// </summary>
        /// <value>
        ///     A message describing the detail.
        /// </value>
        /// =================================================================================================
        public string DetailMessage { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Optional caller-supplied <c>instance</c> URI (overrides factory default).
        /// </summary>
        /// <value>
        ///     The accessed resource URI.
        /// </value>
        /// =================================================================================================
        public string AccessedResourceUri { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Optional caller-supplied extension members merged into the response.
        /// </summary>
        /// <value>
        ///     Information describing the addition.
        /// </value>
        /// =================================================================================================
        public IDictionary<string, object> AdditionInformation { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Ambient <see cref="Microsoft.AspNetCore.Http.HttpContext" /> for the request that produced
        ///     this result, when one is available. Used by
        ///     <see cref="RzR.ResultMessage.Web.Factories.DefaultProblemDetailsResultFactory" /> to
        ///     auto-populate <c>traceId</c> in <c>ProblemDetails.Extensions</c> and may be consumed
        ///     by custom factories for additional correlation data. <see langword="null" /> when the
        ///     call site has no HttpContext (e.g. unit tests, controller extension methods).
        /// </summary>
        /// =================================================================================================
        public HttpContext HttpContext { get; set; }
    }
}