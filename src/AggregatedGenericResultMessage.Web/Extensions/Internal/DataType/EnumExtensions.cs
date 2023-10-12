// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-08 17:18
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-08 17:21
// ***********************************************************************
//  <copyright file="EnumExtensions.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using CodeSource;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.Internal.DataType
{
    /// <summary>
    ///     Enum extensions
    /// </summary>
    /// <remarks></remarks>
    internal static class EnumExtensions
    {
        /// <summary>
        ///     Get int from enum property
        /// </summary>
        /// <param name="source">Enum property</param>
        /// <returns></returns>
        /// <typeparam name="T"></typeparam>
        /// <remarks></remarks>
        [CodeSource("https://github.com/I-RzR-I/DomainCommonExtensions", "RzR",
            "DomainCommonExtensions.CommonExtensions.EnumExtensions", 1)]
        internal static int ToInt<T>(this T source) where T : IConvertible
            => !typeof(T).IsEnum
                ? throw new ArgumentException("T must be an enumerated type")
                : (int)(IConvertible)source;
    }
}