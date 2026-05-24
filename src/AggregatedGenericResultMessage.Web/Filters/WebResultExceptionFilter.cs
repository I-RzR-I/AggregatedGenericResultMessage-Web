// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 07:55
// ***********************************************************************
//  <copyright file="WebResultExceptionFilter.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Mvc.Filters;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Models;
using System.Threading.Tasks;

#endregion

namespace RzR.ResultMessage.Web.Filters
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     MVC exception filter that auto-converts an unhandled <see cref="WebResultException" />
    ///     into a ProblemDetails response built by the configured
    ///     <see cref="ProblemDetailsResultFactory.Current" />. Users can simply
    ///     <c>throw new WebResultException(result)</c> from any controller action and the filter
    ///     handles the conversion — no per-call <c>.AsProblemDetails()</c> call needed.
    ///     <para>
    ///         Implements both <see cref="IAsyncExceptionFilter" /> (preferred by the ASP.NET Core
    ///         pipeline in high-throughput scenarios) and <see cref="IExceptionFilter" /> (retained
    ///         for backward compatibility). When both interfaces are present the runtime always
    ///         dispatches to <see cref="OnExceptionAsync" />.
    ///     </para>
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.IAsyncExceptionFilter"/>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter"/>
    /// =================================================================================================
    public sealed class WebResultExceptionFilter : IAsyncExceptionFilter, IExceptionFilter
    {
        /// <inheritdoc />
        public Task OnExceptionAsync(ExceptionContext context)
        {
            HandleException(context);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void OnException(ExceptionContext context) 
            => HandleException(context);

        private static void HandleException(ExceptionContext context)
        {
            if (context.IsNull() || context.Exception.IsNull())
                return;

            if (!(context.Exception is WebResultException resultException))
                return;

            var statusCode = resultException.StatusCode
                             ?? ResultStatusCodeMapper.Current.Map(resultException.Result, false);

            var instance = resultException.AccessedResourceUri.IfIsMissing(
                context.HttpContext?.Request?.Path.Value);

            var objectResult = ProblemDetailsResultFactory.Current.Create(new ResultProblemDetailsContext
            {
                Result = resultException.Result,
                StatusCode = statusCode,
                HasResponseBody = false,
                Response = null,
                Message = resultException.ProblemTitle,
                DetailMessage = resultException.ProblemDetail,
                AccessedResourceUri = instance,
                AdditionalInformation = resultException.AdditionalInformation,
                HttpContext = context.HttpContext
            });

            context.Result = objectResult;
            context.ExceptionHandled = true;
        }
    }
}