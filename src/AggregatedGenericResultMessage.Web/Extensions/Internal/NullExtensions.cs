// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-08 17:21
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-08 17:22
// ***********************************************************************
//  <copyright file="NullExtensions.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

using CodeSource;

namespace AggregatedGenericResultMessage.Web.Extensions.Internal
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
    }
}