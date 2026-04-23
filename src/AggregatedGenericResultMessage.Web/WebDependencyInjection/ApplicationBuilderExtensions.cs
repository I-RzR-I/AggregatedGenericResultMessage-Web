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
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="app" /> is null.
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

            return app.UseMiddleware<WebResultExceptionMiddleware>();
        }
    }
}