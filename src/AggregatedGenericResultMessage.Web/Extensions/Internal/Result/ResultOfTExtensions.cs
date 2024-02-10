// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-10-11 18:17
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-10-11 18:20
// ***********************************************************************
//  <copyright file="ResultOfTExtensions.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.Internal.DataType;
using AggregatedGenericResultMessage.Web.Models;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.Internal.Result
{
    /// <summary>
    ///     Result &lt;T&gt; extensions
    /// </summary>
    internal static class ResultOfTExtensions
    {
        /// <summary>
        ///     Check if IResult is executed with success
        /// </summary>
        /// <param name="result">IResult&lt;T&gt; result</param>
        /// <param name="resultData">IResult&lt;CheckHttpStatus&gt; result data</param>
        /// <returns></returns>
        internal static bool IsWithSuccess<T>(this IResult<T> result, IResult<CheckHttpStatus> resultData)
            => result.IsSuccess.IsTrue() && resultData.Response.IsSuccess.IsTrue();

        /// <summary>
        ///     Check if IResult is executed with success
        /// </summary>
        /// <param name="result">IResult&lt;T&gt; result</param>
        /// <param name="resultData">IResult&lt;CheckHttpStatus&gt; result data</param>
        /// <returns></returns>
        internal static bool IsWithSuccess<T>(this Result<T> result, IResult<CheckHttpStatus> resultData)
            => result.IsSuccess.IsTrue() && resultData.Response.IsSuccess.IsTrue();
    }
}