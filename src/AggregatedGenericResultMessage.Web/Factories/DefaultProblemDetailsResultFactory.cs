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
using RzR.ResultMessage.Abstractions.Models;
using RzR.ResultMessage.Enums;
using RzR.ResultMessage.Web.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Helpers.Store;
using RzR.ResultMessage.Web.Models;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace RzR.ResultMessage.Web.Factories
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Default <see cref="IProblemDetailsResultFactory" /> that mirrors the historical per-call
    ///     behavior. Subclass and override the protected <c>Resolve*</c> hooks to set global
    ///     defaults for <c>type</c>, <c>title</c>, <c>detail</c>, <c>instance</c>, <c>code</c> or to
    ///     enrich the <c>Extensions</c> dictionary uniformly. Where <see cref="ResultProblemDetailsContext" />
    ///     carries a per-call value - <see cref="ResultProblemDetailsContext.Message" />,
    ///     <see cref="ResultProblemDetailsContext.DetailMessage" />,
    ///     <see cref="ResultProblemDetailsContext.AccessedResourceUri" /> and
    ///     <see cref="ResultProblemDetailsContext.AdditionalInformation" /> - that value always wins over
    ///     the resolved default. <c>code</c> is the exception: the context carries no per-call override
    ///     for it, so it is always factory-resolved through <see cref="ResolveCode" />.
    /// </summary>
    /// <seealso cref="T:RzR.ResultMessage.Web.Abstractions.IProblemDetailsResultFactory"/>
    /// =================================================================================================
    public class DefaultProblemDetailsResultFactory : IProblemDetailsResultFactory
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Immutable) the maximum accepted length of the <c>code</c> member.
        /// </summary>
        /// =================================================================================================
        private const int MaxCodeLength = 64;

        /// <inheritdoc/>
        public virtual ObjectResult Create(ResultProblemDetailsContext context)
        {
            var httpCode = context.StatusCode.ToInt();

            if (context.Result.IsNotNull() && context.Result.IsSuccess.IsTrue() && httpCode.IsSuccessHttpStatusRange())
                return new ObjectResult(context.HasResponseBody ? context.Response : null) { StatusCode = httpCode };

            var problem = new ResultMessageProblemDetails
            {
                Status = httpCode,
                Title = context.Message.IfIsMissing(ResolveTitle(context)),
                Type = ResolveType(context),
                Detail = context.DetailMessage.IfIsMissing(ResolveDetail(context)),
                Instance = context.AccessedResourceUri.IfIsMissing(ResolveInstance(context)),
                Code = SanitizeCode(ResolveCode(context))
            };

            ApplyExtensions(problem, context);

            problem.Code = SanitizeCode(problem.Code);

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
        ///     Resolves the default machine-readable <c>code</c>. Defaults to the <c>Key</c> of the very
        ///     same message that supplies <c>title</c> and <c>detail</c>, and to <see langword="null" />
        ///     when that message carries no key.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     A string.
        /// </returns>
        /// =================================================================================================
        protected virtual string ResolveCode(ResultProblemDetailsContext context)
        {
            try
            {
                var messages = context.Result?.Messages;

                if (messages.IsNull())
                    return null;

                foreach (var message in messages!)
                {
                    if (message?.MessageType != MessageType.Exception)
                        return message?.Key;
                }

                return messages.FirstOrDefault()?.Key;
            }
            catch
            {
                return null;
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Merges caller-supplied <see cref="ResultProblemDetailsContext.AdditionalInformation" /> and
        ///     the canonical <c>ResultMessages</c> entry into the problem-details
        ///     <c>Extensions</c> dictionary. Override to inject additional global extensions
        ///     (e.g. <c>traceId</c>).
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="context">The context.</param>
        /// =================================================================================================
        protected virtual void ApplyExtensions(ResultMessageProblemDetails problem, ResultProblemDetailsContext context)
        {
            if (context.AdditionalInformation.IsNullOrEmptyEnumerable().IsFalse())
            {
                foreach (var kvp in context.AdditionalInformation!)
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

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Validates a resolved <c>code</c> candidate. Accepts it unchanged when it is non-blank, at
        ///     most <see cref="MaxCodeLength" /> characters long and composed only of <c>[A-Za-z0-9._-]</c>;
        ///     otherwise returns <see langword="null" /> so the member is omitted from the response.
        /// </summary>
        /// <param name="code">The resolved code candidate.</param>
        /// <returns>
        ///     A string.
        /// </returns>
        /// =================================================================================================
        private static string SanitizeCode(string code)
        {
            if (code.IsMissing() || code.Length > MaxCodeLength)
                return null;

            foreach (var symbol in code)
            {
                var isAllowed = (symbol >= 'A' && symbol <= 'Z')
                                || (symbol >= 'a' && symbol <= 'z')
                                || (symbol >= '0' && symbol <= '9')
                                || symbol == '.' || symbol == '_' || symbol == '-';

                if (isAllowed.IsFalse())
                    return null;
            }

            return code;
        }
    }
}