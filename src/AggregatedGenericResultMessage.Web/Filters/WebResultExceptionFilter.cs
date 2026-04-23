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

#endregion

namespace RzR.ResultMessage.Web.Filters
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     MVC <see cref="IExceptionFilter" /> that auto-converts an unhandled
    ///     <see cref="WebResultException" /> into a ProblemDetails response built by the
    ///     configured <see cref="ProblemDetailsResultFactory.Current" />. User can simply
    ///     <c>throw new ResultException(result)</c> from any controller action and the response
    ///     pipeline does the conversion, no need for per-call <c>.AsProblemDetails()</c> invoke.
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter" />
    /// =================================================================================================
    public sealed class WebResultExceptionFilter : IExceptionFilter
    {
        /// <inheritdoc />
        public void OnException(ExceptionContext context)
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
                AdditionInformation = resultException.AdditionInformation
            });

            context.Result = objectResult;
            context.ExceptionHandled = true;
        }
    }
}