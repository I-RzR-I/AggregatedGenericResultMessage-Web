// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2024-12-25 13:06
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 13:40
// ***********************************************************************
//  <copyright file="StringExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using CodeSource;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.Internal.DataType
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     A string extensions.
    /// </summary>
    /// =================================================================================================
    internal static class StringExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A string extension method that if is missing.
        /// </summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        ///     A string.
        /// </returns>
        /// =================================================================================================
        internal static string IfIsMissing(this string source, string defaultValue)
        {
            if (source.IsNull() || source.IsMissing())
                return defaultValue;

            return source;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     A string extension method that query if 'source' is missing.
        /// </summary>
        /// <param name="source">The source to act on.</param>
        /// <returns>
        ///     True if missing, false if not.
        /// </returns>
        /// =================================================================================================
        [CodeSource("https://github.com/I-RzR-I/DomainCommonExtensions", "RzR",
            "DomainCommonExtensions.DataTypeExtensions.StringExtensions.IsMissing", 1)]
        internal static bool IsMissing(this string source) => string.IsNullOrWhiteSpace(source);
    }
}