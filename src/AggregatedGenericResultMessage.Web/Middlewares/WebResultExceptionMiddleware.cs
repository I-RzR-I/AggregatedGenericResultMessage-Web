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
using RzR.ResultMessage.Web.Abstractions;
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
    ///     request pipeline and renders a ProblemDetails response built by the registered
    ///     <see cref="IProblemDetailsResultFactory" />, or by
    ///     <see cref="ProblemDetailsResultFactory.Current" /> when the container has none.
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
        ///     Message reported when the host has no MVC services available to render the response.
        /// </summary>
        /// =================================================================================================
        internal const string MvcServicesRequiredMessage =
            "WebResultExceptionMiddleware requires MVC services. Call services.AddControllers() (or AddMvcCore) before UseResultExceptionMiddleware().";

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
        ///     Invokes the middleware. A failure while rendering the ProblemDetails response is
        ///     contained and degraded to a bare status code instead of escaping the pipeline.
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

                try
                {
                    if (ex is WebResultException resultException)
                        await WriteResultExceptionAsync(context, resultException).ConfigureAwait(false);
                    else
                        await WriteUnhandledExceptionAsync(context, ex).ConfigureAwait(false);
                }
                catch (Exception renderException)
                {
                    NotifyRenderFailure(context, renderException);
                    WriteFallbackStatusCode(context, ResolveFallbackStatusCode(ex));
                }
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

            var objectResult = ResolveFactory(context).Create(new ResultProblemDetailsContext
            {
                Result = ex.Result,
                StatusCode = statusCode,
                HasResponseBody = false,
                Response = null,
                Message = ex.ProblemTitle,
                DetailMessage = ex.ProblemDetail,
                AccessedResourceUri = instance,
                AdditionalInformation = ex.AdditionalInformation,
                HttpContext = context
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
                .WithError(
                    _options.IncludeExceptionMessageInDetail
                        ? ex.Message
                        : _options.DefaultUnhandledTitle,
                    _options.DefaultUnhandledErrorCode);

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

            var objectResult = ResolveFactory(context).Create(new ResultProblemDetailsContext
            {
                Result = failureResult,
                StatusCode = _options.DefaultUnhandledStatusCode,
                HasResponseBody = false,
                Response = null,
                Message = _options.DefaultUnhandledTitle,
                DetailMessage = detail,
                AccessedResourceUri = context.Request?.Path.Value,
                AdditionalInformation = extras,
                HttpContext = context
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
                throw new InvalidOperationException(MvcServicesRequiredMessage);

            var actionContext = new ActionContext(
                context,
                context.GetRouteData() ?? new RouteData(),
                new ActionDescriptor());

            await executor!.ExecuteAsync(actionContext, objectResult).ConfigureAwait(false);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Resolves the factory from the request container and falls back to
        ///     <see cref="ProblemDetailsResultFactory.Current" /> when none is registered.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     A never null <see cref="IProblemDetailsResultFactory" />.
        /// </returns>
        /// =================================================================================================
        private static IProblemDetailsResultFactory ResolveFactory(HttpContext context)
            => context?.RequestServices?.GetService<IProblemDetailsResultFactory>()
               ?? ProblemDetailsResultFactory.Current;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Determines the status code written when the ProblemDetails render fails.
        /// </summary>
        /// <param name="ex">The originally caught exception.</param>
        /// <returns>
        ///     The fallback status code.
        /// </returns>
        /// =================================================================================================
        private int ResolveFallbackStatusCode(Exception ex)
            => (int)((ex as WebResultException)?.StatusCode ?? _options.DefaultUnhandledStatusCode);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Reports a render failure to the observability hook without letting it escape.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderException">The exception thrown while rendering the response.</param>
        /// =================================================================================================
        private void NotifyRenderFailure(HttpContext context, Exception renderException)
        {
            try
            {
                _options.OnException?.Invoke(renderException, context);
            }
            catch
            {
                // A failing hook must not prevent the fallback status code below from being written.
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Writes a bodiless response carrying <paramref name="statusCode" /> as a last resort.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="statusCode">The status code to write.</param>
        /// =================================================================================================
        private static void WriteFallbackStatusCode(HttpContext context, int statusCode)
        {
            try
            {
                if (context.Response.HasStarted)
                    return;

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
            }
            catch
            {
                // The response is no longer writable, so nothing can be emitted for this request.
            }
        }
    }
}
