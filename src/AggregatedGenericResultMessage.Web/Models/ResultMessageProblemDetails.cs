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
using System.ComponentModel;

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

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets or sets a short machine-readable error code that identifies the specific failure
        ///     condition, as opposed to the HTTP status which identifies the class of failure.
        /// </summary>
        /// <value>
        ///     The machine-readable error code, or <see langword="null" /> when the result carries none.
        /// </value>
        /// =================================================================================================
        public string Code { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Determines whether <see cref="Code" /> is written to the serialized payload.
        /// </summary>
        /// <returns>
        ///     True if <see cref="Code" /> should be serialized, false if it should be omitted.
        /// </returns>
        /// <remarks>
        ///     This is a Newtonsoft.Json naming convention resolved by reflection, so it requires no reference to
        ///     that package. Together with the net5.0+ converter's own guard it makes omission of a blank
        ///     <see cref="Code" /> guaranteed on every serialization path.
        /// </remarks>
        /// =================================================================================================
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeCode() => !string.IsNullOrWhiteSpace(Code);
    }
}