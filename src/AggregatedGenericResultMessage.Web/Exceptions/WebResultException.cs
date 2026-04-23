// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 07:57
// ***********************************************************************
//  <copyright file="WebResultException.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;

#endregion

namespace RzR.ResultMessage.Web.Exceptions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Custom exception that carries an <see cref="RzR.ResultMessage.Abstractions.IResult" /> together with optional
    ///     ProblemDetails-shaping data. When thrown from controller code and the
    ///     <c>WebResultExceptionFilter</c> is registered, the response is automatically
    ///     converted to a ProblemDetails payload via the configured
    ///     <see cref="RzR.ResultMessage.Web.Abstractions.IProblemDetailsResultFactory" />,
    ///     so no need to invoke <c>.AsProblemDetails()</c>.
    /// </summary>
    /// <seealso cref="T:Exception"/>
    /// =================================================================================================
    public class WebResultException : Exception
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     The wrapped result.
        /// </summary>
        /// <value>
        ///     The result.
        /// </value>
        /// =================================================================================================
        public IResult Result { get; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Optional) Explicit HTTP status code. When <c>null</c>, the filter resolves the status
        ///     code via <c>ResultStatusCodeMapper.Current.Map(Result, hasResponseBody:false)</c>. 
        /// </summary>
        /// <value>
        ///     The status code.
        /// </value>
        /// =================================================================================================
        public HttpStatusCode? StatusCode { get; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Optional) Per-call problem-details title override.
        /// </summary>
        /// <value>
        ///     The problem title.
        /// </value>
        /// =================================================================================================
        public string ProblemTitle { get; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Optional) Per-call problem-details detail override.
        /// </summary>
        /// <value>
        ///     The problem detail.
        /// </value>
        /// =================================================================================================
        public string ProblemDetail { get; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Optional) Per-call problem-details instance (URI) override.
        /// </summary>
        /// <value>
        ///     The accessed resource URI.
        /// </value>
        /// =================================================================================================
        public string AccessedResourceUri { get; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Optional) Per-call additional ProblemDetails extensions.
        /// </summary>
        /// <value>
        ///     Information describing the addition.
        /// </value>
        /// =================================================================================================
        public IDictionary<string, object> AdditionInformation { get; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Initializes a new <see cref="WebResultException" /> wrapping the given
        ///     <paramref name="result" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="result" /> is null.
        /// </exception>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">
        ///     (Optional)
        ///     The status code.
        /// </param>
        /// <param name="message">(Optional) The message.</param>
        /// <param name="detailMessage">(Optional) Message describing the detail.</param>
        /// <param name="accessedResourceUri">
        ///     (Optional)
        ///     The accessed resource URI.
        /// </param>
        /// <param name="additionInformation">
        ///     (Optional)
        ///     Information describing the addition.
        /// </param>
        /// <param name="innerException">(Optional) The inner exception.</param>
        /// =================================================================================================
        public WebResultException(
            IResult result,
            HttpStatusCode? statusCode = null,
            string message = null,
            string detailMessage = null,
            string accessedResourceUri = null,
            IDictionary<string, object> additionInformation = null,
            Exception innerException = null)
            : base(message ?? "Result-bound failure raised as exception.", innerException)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
            StatusCode = statusCode;
            ProblemTitle = message;
            ProblemDetail = detailMessage;
            AccessedResourceUri = accessedResourceUri;
            AdditionInformation = additionInformation;
        }
    }
}