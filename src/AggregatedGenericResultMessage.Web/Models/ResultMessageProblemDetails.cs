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
using System;
using System.Collections.Generic;

// ReSharper disable CollectionNeverQueried.Global

#endregion

namespace AggregatedGenericResultMessage.Web.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Information about the result message problem.
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ProblemDetails"/>
    /// =================================================================================================
    public class ResultMessageProblemDetails : ProblemDetails
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Gets the extensions.
        /// </summary>
        /// <value>
        ///     The extensions.
        /// </value>
        /// =================================================================================================
        public IDictionary<string, object> Extensions { get; } = new Dictionary<string, object>(StringComparer.Ordinal);
    }
}