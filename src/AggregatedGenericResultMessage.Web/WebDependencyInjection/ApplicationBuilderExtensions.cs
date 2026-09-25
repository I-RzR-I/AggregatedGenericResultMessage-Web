// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 08:21
// ***********************************************************************
//  <copyright file="ApplicationBuilderExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Middlewares;
using System;

#endregion

namespace RzR.ResultMessage.Web.WebDependencyInjection
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     An application builder extensions.
    /// </summary>
    /// =================================================================================================
    public static class ApplicationBuilderExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Adds the <see cref="WebResultExceptionMiddleware" /> to the request pipeline so any
        ///     unhandled <see cref="WebResultException" /> is auto-converted to a ProblemDetails
        ///     response. 
        ///     Place it early in the pipeline (before <c>UseRouting</c>) so it covers
        ///     middleware-level exceptions as well as MVC actions.
        ///     Verifies at startup that MVC services are available, so a misconfigured host fails
        ///     fast instead of at the first unhandled request exception.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="app" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when MVC services required to render the response are not registered.
        /// </exception>
        /// <param name="app">The app to act on.</param>
        /// <returns>
        ///     An IApplicationBuilder.
        /// </returns>
        /// =================================================================================================
        public static IApplicationBuilder UseResultExceptionMiddleware(this IApplicationBuilder app)
        {
            if (app.IsNull())
                throw new ArgumentNullException(nameof(app));

            var executor = app.ApplicationServices?.GetService<IActionResultExecutor<ObjectResult>>();
            if (executor.IsNull())
                throw new InvalidOperationException(WebResultExceptionMiddleware.MvcServicesRequiredMessage);

            return app.UseMiddleware<WebResultExceptionMiddleware>();
        }
    }
}