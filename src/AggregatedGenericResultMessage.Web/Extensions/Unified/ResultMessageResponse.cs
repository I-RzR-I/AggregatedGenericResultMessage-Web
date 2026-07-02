// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author            : RzR
//  Created           : 02-07-2026 19:07
// 
//  Last Modified By : RzR
//  Last Modified On : 02-07-2026 20:08
//  ***********************************************************************
//  <copyright file="ResultMessageResponse.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

#region U S I N G

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif

#endregion

namespace RzR.ResultMessage.Web.Extensions.Unified
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     A single response value that is simultaneously a valid MVC <see cref="IActionResult" /> and
    ///     (net6+) a valid Minimal-API <c>Microsoft.AspNetCore.Http.IResult</c>, wrapping the same
    ///     <see cref="ObjectResult" /> produced by
    ///     <see cref="RzR.ResultMessage.Web.Helpers.ResultProblemDetailsHelper" />. The hosting framework
    ///     dispatches to whichever interface member it understands (<see cref="ExecuteResultAsync" />
    ///     for MVC controllers, <c>ExecuteAsync</c> for Minimal-API endpoints) — no host detection
    ///     is performed here, so both code paths render byte-identical responses.
    /// </summary>
    /// =================================================================================================
    public sealed class ResultMessageResponse : IActionResult
#if NET6_0_OR_GREATER
        ,
        IResult
#endif
    {
        private readonly ObjectResult _objectResult;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="objectResult">The <see cref="ObjectResult" /> to wrap.</param>
        /// =================================================================================================
        internal ResultMessageResponse(ObjectResult objectResult) 
            => _objectResult = objectResult ?? throw new ArgumentNullException(nameof(objectResult));

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     MVC dispatch: delegates to the wrapped <see cref="ObjectResult" /> so controller actions
        ///     render exactly as they would returning the <see cref="ObjectResult" /> directly.
        /// </summary>
        /// <param name="context">The action context.</param>
        /// <returns>
        ///     A Task.
        /// </returns>
        /// =================================================================================================
        public Task ExecuteResultAsync(ActionContext context) 
            => _objectResult.ExecuteResultAsync(context);

#if NET6_0_OR_GREATER
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Minimal-API dispatch: reuses <see cref="MinimalApiResultWrapper.Wrap" /> — the same
        ///     terminal-wrap logic used by <see cref="RzR.ResultMessage.Web.Extensions.MinimalApi.ResultToHttpResult" />
        ///     — so a handler returning this value renders the same body/status as the dedicated
        ///     Minimal-API extensions.
        /// </summary>
        /// <param name="httpContext">The HTTP context for the current request.</param>
        /// <returns>
        ///     A Task.
        /// </returns>
        /// =================================================================================================
        public Task ExecuteAsync(HttpContext httpContext)
            => MinimalApiResultWrapper.Wrap(_objectResult.Value, _objectResult.StatusCode ?? StatusCodes.Status200OK)
                .ExecuteAsync(httpContext);
#endif
    }
}