// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-08 17:21
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 17:41
// ***********************************************************************
//  <copyright file="NullExtensions.cs" company="RzR SOFT & TECH">
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
    /// <summary>
    ///     Null extensions
    /// </summary>
    /// <remarks></remarks>
    internal static class NullExtensions
    {
        /// <summary>
        ///     Is null
        /// </summary>
        /// <param name="obj">Source data</param>
        /// <returns></returns>
        [CodeSource("https://github.com/I-RzR-I/DomainCommonExtensions", "RzR", 
            "DomainCommonExtensions.CommonExtensions.NullExtensions.IsNull", 1)]
        internal static bool IsNull(this object obj) => obj == null;

        /// <summary>
        ///     Is not null
        /// </summary>
        /// <param name="obj">Object to be checked</param>
        /// <returns></returns>
        [CodeSource("https://github.com/I-RzR-I/DomainCommonExtensions", "RzR", 
            "DomainCommonExtensions.CommonExtensions.NullExtensions.IsNotNull", 1)]
        public static bool IsNotNull(this object obj) => obj != null;
    }
}