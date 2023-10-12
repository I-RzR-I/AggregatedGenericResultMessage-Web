// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-10-11 18:01
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-10-11 18:10
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
    ///     Result &lt;CheckHttpStatus&gt; extensions
    /// </summary>
    internal static class ResultOfCheckHttpStatusExtensions
    {
        /// <summary>
        ///     Check if IResult&lt;CheckHttpStatus&gt; is executed with no success/error
        /// </summary>
        /// <param name="result">IResult&lt;CheckHttpStatus&gt; result</param>
        /// <returns></returns>
        internal static bool IsNoSuccess(this IResult<CheckHttpStatus> result)
            => result.IsSuccess.IsFalse() || result.Response.IsError.IsTrue();

        /// <summary>
        ///     Check if IResult&lt;CheckHttpStatus&gt; is executed with success
        /// </summary>
        /// <param name="result">IResult&lt;CheckHttpStatus&gt; result</param>
        /// <returns></returns>
        internal static bool IsSuccess(this IResult<CheckHttpStatus> result)
            => result.IsSuccess.IsTrue() && result.Response.IsSuccess.IsTrue();
    }
}