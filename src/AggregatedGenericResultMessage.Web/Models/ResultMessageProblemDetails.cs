// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2024-12-24 22:03
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 13:56
// ***********************************************************************
//  <copyright file="ResultMessageProblemDetails.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Web.Helpers;

#if NETSTANDARD2_1
using System;
using System.Collections.Generic;
#endif

#if NET5_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

// ReSharper disable CollectionNeverQueried.Global

#endregion

namespace RzR.ResultMessage.Web.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Information about the result message problem.
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ProblemDetails"/>
    /// =================================================================================================
#if NET5_0_OR_GREATER
    [JsonConverter(typeof(ResultMessageProblemDetailsConverter))]
#endif
    public class ResultMessageProblemDetails : ProblemDetails
    {
#if NETSTANDARD2_1
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets the extensions.
        /// </summary>
        /// <value>
        ///     The extensions.
        /// </value>
        /// =================================================================================================
        public IDictionary<string, object> Extensions { get; } = new Dictionary<string, object>(StringComparer.Ordinal);
#endif
    }
}