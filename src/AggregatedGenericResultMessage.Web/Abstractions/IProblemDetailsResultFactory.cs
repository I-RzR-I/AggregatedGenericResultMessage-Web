// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:57
// ***********************************************************************
//  <copyright file="IProblemDetailsResultFactory.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Web.Models;

#endregion

namespace RzR.ResultMessage.Web.Abstractions
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Strategy that converts a <see cref="ResultProblemDetailsContext" /> into the
    ///     <see cref="ResultMessageProblemDetails" /> body and the wrapping
    ///     <see cref="ObjectResult" />. Register a custom implementation via
    ///     <c>services.AddProblemDetailsResultFactory&lt;TFactory&gt;()</c> to configure
    ///     <c>type</c> / <c>title</c> / <c>instance</c> / extension defaults globally instead of
    ///     supplying them at every call site.
    /// </summary>
    /// =================================================================================================
    public interface IProblemDetailsResultFactory
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Builds the problem-details payload (or, on success-with-body, a payload-only
        ///     <see cref="ObjectResult" />) for the given <paramref name="context" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     An ObjectResult.
        /// </returns>
        /// =================================================================================================
        ObjectResult Create(ResultProblemDetailsContext context);
    }
}