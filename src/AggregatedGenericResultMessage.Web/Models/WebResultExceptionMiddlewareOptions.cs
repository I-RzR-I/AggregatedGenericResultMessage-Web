// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-23 07:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 08:20
// ***********************************************************************
//  <copyright file="WebResultExceptionMiddlewareOptions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Http;
using RzR.ResultMessage.Web.Middlewares;
using System;
using System.Net;

#endregion

namespace RzR.ResultMessage.Web.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Options controlling <see cref="WebResultExceptionMiddleware" />'s handling
    ///      of non-<see cref="Exceptions.WebResultException" />
    ///     exceptions.
    /// </summary>
    /// =================================================================================================
    public sealed class WebResultExceptionMiddlewareOptions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Status code used when the middleware catches an exception that is NOT a
        ///     <see cref="Exceptions.WebResultException" />. Defaults to <c>500 InternalServerError</c>. 
        /// </summary>
        /// <value>
        ///     The default unhandled status code.
        /// </value>
        /// =================================================================================================
        public HttpStatusCode DefaultUnhandledStatusCode { get; set; } = HttpStatusCode.InternalServerError;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Generic title applied when the caught exception is not a
        ///     <see cref="Exceptions.WebResultException" />. Defaults to <c>"Unhandled exception"</c>. 
        /// </summary>
        /// <value>
        ///     The default unhandled title.
        /// </value>
        /// =================================================================================================
        public string DefaultUnhandledTitle { get; set; } = "Unhandled exception";

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     When <c>true</c>, the exception's <see cref="Exception.Message" /> is propagated to the
        ///     ProblemDetails <c>detail</c> field. When <c>false</c> (default — safer for production)
        ///     a generic message is used instead.
        /// </summary>
        /// <value>
        ///     True if include exception message in detail, false if not.
        /// </value>
        /// =================================================================================================
        public bool IncludeExceptionMessageInDetail { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     When <c>true</c>, the exception's full <see cref="Exception.ToString()" /> output
        ///     (including stack trace) is added to the ProblemDetails extensions under the
        ///     <c>exception</c> key. <strong>Do not enable in production.</strong>
        /// </summary>
        /// <value>
        ///     True if include exception details in extensions, false if not.
        /// </value>
        /// =================================================================================================
        public bool IncludeExceptionDetailsInExtensions { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Optional callback invoked for every caught exception (including
        ///     <see cref="Exceptions.WebResultException" />). Useful for logging hooks that want access
        ///     to
        ///     both the exception and the originating request.
        /// </summary>
        /// <value>
        ///     The on exception.
        /// </value>
        /// =================================================================================================
        public Action<Exception, HttpContext> OnException { get; set; }
    }
}