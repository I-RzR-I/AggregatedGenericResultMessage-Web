// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:59
// ***********************************************************************
//  <copyright file="DefaultProblemDetailsResultFactory.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Web.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Helpers.Store;
using RzR.ResultMessage.Web.Models;

#endregion

namespace RzR.ResultMessage.Web.Factories
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Default <see cref="IProblemDetailsResultFactory" /> that mirrors the historical per-call
    ///     behavior. Subclass and override the protected <c>Resolve*</c> hooks to set global
    ///     defaults for <c>type</c>, <c>title</c>, <c>detail</c>, <c>instance</c> or to enrich the <c>
    ///     Extensions</c> dictionary uniformly. Per-call values from <see cref="ResultProblemDetailsContext" />
    ///     always win over the resolved defaults.
    /// </summary>
    /// <seealso cref="T:RzR.ResultMessage.Web.Abstractions.IProblemDetailsResultFactory"/>
    /// =================================================================================================
    public class DefaultProblemDetailsResultFactory : IProblemDetailsResultFactory
    {
        /// <inheritdoc/>
        public virtual ObjectResult Create(ResultProblemDetailsContext context)
        {
            var httpCode = context.StatusCode.ToInt();

            // Success + a successful HTTP code => write the payload (or null) directly.
            if (context.Result.IsNotNull() && context.Result.IsSuccess.IsTrue() && httpCode.IsSuccessHttpStatus())
                return new ObjectResult(context.HasResponseBody ? context.Response : null) { StatusCode = httpCode };

            var problem = new ResultMessageProblemDetails
            {
                Status = httpCode,
                Title = context.Message.IfIsMissing(ResolveTitle(context)),
                Type = ResolveType(context),
                Detail = context.DetailMessage.IfIsMissing(ResolveDetail(context)),
                Instance = context.AccessedResourceUri.IfIsMissing(ResolveInstance(context))
            };

            ApplyExtensions(problem, context);

            return new ObjectResult(problem) { StatusCode = httpCode };
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Resolves the <c>type</c> URI for the current status code. Defaults to the RFC dictionary
        ///     lookup with an <c>about:blank</c> fallback.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     A string.
        /// </returns>
        /// =================================================================================================
        protected virtual string ResolveType(ResultProblemDetailsContext context)
        {
            var key = context.StatusCode.ToString();

            return RfcTypeHttpCodeDictionary.RfcHttpStatusCodeInfo.TryGetValue(key, out var rfcType)
                ? rfcType
                : "about:blank";
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Resolves the default title. Defaults to the first message <c>Info</c> on the result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     A string.
        /// </returns>
        /// =================================================================================================
        protected virtual string ResolveTitle(ResultProblemDetailsContext context)
            => context.Result?.GetFirstMessageWithDetails()?.Info;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Resolves the default detail. Defaults to the first message's <c>ToString()</c>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     A string.
        /// </returns>
        /// =================================================================================================
        protected virtual string ResolveDetail(ResultProblemDetailsContext context)
            => context.Result?.GetFirstMessageWithDetails()?.ToString();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Resolves the default <c>instance</c> URI. Defaults to <see langword="null" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     A string.
        /// </returns>
        /// =================================================================================================
        protected virtual string ResolveInstance(ResultProblemDetailsContext context) => null;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Merges caller-supplied <see cref="ResultProblemDetailsContext.AdditionInformation" /> and
        ///     the canonical <c>ResultMessages</c> entry into the problem-details
        ///     <c>Extensions</c> dictionary. Override to inject additional global extensions
        ///     (e.g. <c>traceId</c>).
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="context">The context.</param>
        /// =================================================================================================
        protected virtual void ApplyExtensions(ResultMessageProblemDetails problem, ResultProblemDetailsContext context)
        {
            if (context.AdditionInformation.IsNullOrEmptyEnumerable().IsFalse())
            {
                foreach (var kvp in context.AdditionInformation!)
                    problem.Extensions[kvp.Key] = kvp.Value;
            }

            if (context.Result.IsNotNull())
                problem.Extensions["ResultMessages"] = context.Result.Messages;

            if (context.HttpContext.IsNotNull()
                && !problem.Extensions.ContainsKey("traceId"))
            {
                problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            }
        }
    }
}