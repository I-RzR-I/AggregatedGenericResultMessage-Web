// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-10-11 17:49
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-10-11 18:01
// ***********************************************************************
//  <copyright file="StatusCodeExtensions.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.Internal.Http
{
    /// <summary>
    ///     HTTP status code extensions
    /// </summary>
    internal static class StatusCodeExtensions
    {
        /// <summary>
        ///     Is OK HTTP status code
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns></returns>
        internal static bool IsOk(this HttpStatusCode statusCode)
            => statusCode == HttpStatusCode.OK;

        /// <summary>
        ///     Is NoContent HTTP status code
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns></returns>
        internal static bool IsOkNoContent(this HttpStatusCode statusCode)
            => statusCode == HttpStatusCode.NoContent;
    }
}