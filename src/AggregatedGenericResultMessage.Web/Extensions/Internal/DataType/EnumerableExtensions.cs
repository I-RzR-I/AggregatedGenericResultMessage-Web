// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2024-12-25 13:51
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 14:28
// ***********************************************************************
//  <copyright file="EnumerableExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using CodeSource;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.Internal.DataType
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     An enumerable extensions.
    /// </summary>
    /// =================================================================================================
    internal static class EnumerableExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     An IEnumerable&lt;T&gt; extension method that query if 'source' is null or empty
        ///     enumerable.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="source">The source to act on.</param>
        /// <returns>
        ///     True if null or empty enumerable, false if not.
        /// </returns>
        /// =================================================================================================
        [CodeSource("https://github.com/I-RzR-I/DomainCommonExtensions", "RzR",
            "DomainCommonExtensions.ArraysExtensions.EnumerableExtensions.IsNullOrEmptyEnumerable", 1)]
        internal static bool IsNullOrEmptyEnumerable<T>(this IEnumerable<T> source)
            => source.IsNull() || !source.Any();
    }
}