// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-10-11 17:14
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-10-11 17:14
// ***********************************************************************
//  <copyright file="BoolExtensions.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace AggregatedGenericResultMessage.Web.Extensions.Internal.DataType
{
    /// <summary>
    ///     Boolean extensions
    /// </summary>
    internal static class BoolExtensions
    {
        /// <summary>
        ///     Check if source value is equals with true
        /// </summary>
        /// <param name="source">Source bool value to be checked</param>
        /// <returns></returns>
        internal static bool IsTrue(this bool source)
            => source.Equals(true);

        /// <summary>
        ///     Check if source value is equals with true
        /// </summary>
        /// <param name="source">Source bool value to be checked</param>
        /// <returns></returns>
        internal static bool IsTrue(this bool? source)
            => source == true;

        /// <summary>
        ///     Check if source value is equals with false
        /// </summary>
        /// <param name="source">Source bool value to be checked</param>
        /// <returns></returns>
        internal static bool IsFalse(this bool source)
            => source != true;

        /// <summary>
        ///     Check if source value is equals with false
        /// </summary>
        /// <param name="source">Source bool value to be checked</param>
        /// <returns></returns>
        internal static bool IsFalse(this bool? source)
            => source != true;
    }
}