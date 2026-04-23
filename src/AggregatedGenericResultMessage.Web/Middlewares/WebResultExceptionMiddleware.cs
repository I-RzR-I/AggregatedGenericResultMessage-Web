// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 08:04
// ***********************************************************************
//  <copyright file="WebResultExceptionMiddleware.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RzR.ResultMessage.Extensions.Result;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace RzR.ResultMessage.Web.Middlewares
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     ASP.NET Core middleware that catches <strong>any</strong> unhandled exception in the
    ///     request pipeline and renders a ProblemDetails response built by the configured
    ///     <see cref="ProblemDetailsResultFactory.Current" />.
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="WebResultException" /> - the wrapped result drives status code
    ///             (via <see cref="ResultStatusCodeMapper.Current" /> when not provided) and per-call ProblemDetails overrides.
    ///         </item>
    ///         <item>
    ///             Any other <see cref="Exception" /> - mapped to a generic failure result and
    ///             rendered with <see cref="WebResultExceptionMiddlewareOptions.DefaultUnhandledStatusCode" />
    ///             (defaults to <c>500</c>). Exception details are surfaced only when the
    ///             corresponding options flags are enabled.
    ///         </item>
    ///     </list>
    ///     Acts as a pipeline-wide counterpart to the MVC <c>WebResultExceptionFilter</c>.
    /// </summary>
    /// =================================================================================================
    public sealed class WebResultExceptionMiddleware
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Immutable) the next.
        /// </summary>
        /// =================================================================================================
        private readonly RequestDelegate _next;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Immutable) options for controlling the operation.
        /// </summary>
        /// =================================================================================================
        private readonly WebResultExceptionMiddlewareOptions _options;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Initializes a new <see cref="WebResultExceptionMiddleware" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="next">The next.</param>
        /// <param name="options">(Optional) Options for controlling the operation.</param>
        /// =================================================================================================
        public WebResultExceptionMiddleware(
            RequestDelegate next,
            IOptions<WebResultExceptionMiddlewareOptions> options = null)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options?.Value ?? new WebResultExceptionMiddlewareOptions();
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Invokes the middleware.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     A Task.
        /// </returns>
        /// =================================================================================================
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                    throw;

                _options.OnException?.Invoke(ex, context);

                if (ex is WebResultException resultException)
                    await WriteResultExceptionAsync(context, resultException).ConfigureAwait(false);
                else
                    await WriteUnhandledExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Writes a result exception asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>
        ///     A Task.
        /// </returns>
        /// =================================================================================================
        private static async Task WriteResultExceptionAsync(HttpContext context, WebResultException ex)
        {
            var statusCode = ex.StatusCode
                             ?? ResultStatusCodeMapper.Current.Map(ex.Result, false);

            var instance = ex.AccessedResourceUri.IfIsMissing(context.Request?.Path.Value);

            var objectResult = ProblemDetailsResultFactory.Current.Create(new ResultProblemDetailsContext
            {
                Result = ex.Result,
                StatusCode = statusCode,
                HasResponseBody = false,
                Response = null,
                Message = ex.ProblemTitle,
                DetailMessage = ex.ProblemDetail,
                AccessedResourceUri = instance,
                AdditionInformation = ex.AdditionInformation
            });

            await ExecuteResultAsync(context, objectResult).ConfigureAwait(false);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Writes an unhandled exception asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>
        ///     A Task.
        /// </returns>
        /// =================================================================================================
        private async Task WriteUnhandledExceptionAsync(HttpContext context, Exception ex)
        {
            var failureResult = new Result { IsSuccess = false }
                .WithError(_options.IncludeExceptionMessageInDetail
                    ? ex.Message
                    : _options.DefaultUnhandledTitle);

            var detail = _options.IncludeExceptionMessageInDetail
                ? ex.Message
                : "An unexpected error occurred while processing the request.";

            IDictionary<string, object> extras = null;
            if (_options.IncludeExceptionDetailsInExtensions)
            {
                extras = new Dictionary<string, object>()
                {
                    ["exception"] = ex.ToString(), 
                    ["exceptionType"] = ex.GetType().FullName
                };
            }

            var objectResult = ProblemDetailsResultFactory.Current.Create(new ResultProblemDetailsContext
            {
                Result = failureResult,
                StatusCode = _options.DefaultUnhandledStatusCode,
                HasResponseBody = false,
                Response = null,
                Message = _options.DefaultUnhandledTitle,
                DetailMessage = detail,
                AccessedResourceUri = context.Request?.Path.Value,
                AdditionInformation = extras
            });

            await ExecuteResultAsync(context, objectResult).ConfigureAwait(false);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Executes the 'result' asynchronous operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the requested operation is invalid.
        /// </exception>
        /// <param name="context">The context.</param>
        /// <param name="objectResult">The object result.</param>
        /// <returns>
        ///     A Task.
        /// </returns>
        /// =================================================================================================
        private static async Task ExecuteResultAsync(HttpContext context, ObjectResult objectResult)
        {
            context.Response.Clear();

            var executor = context.RequestServices?.GetService<IActionResultExecutor<ObjectResult>>();
            if (executor.IsNull())
            {
                throw new InvalidOperationException(
                    "ResultExceptionMiddleware requires MVC services. Call services.AddControllers() (or AddMvcCore) before UseResultExceptionMiddleware().");
            }

            var actionContext = new ActionContext(
                context,
                context.GetRouteData() ?? new RouteData(),
                new ActionDescriptor());

            await executor!.ExecuteAsync(actionContext, objectResult).ConfigureAwait(false);
        }
    }
}