// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-10-11 17:41
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 17:41
// ***********************************************************************
//  <copyright file="IntExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Http;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.Internal.DataType
{
    /// <summary>
    ///     Int32 extensions
    /// </summary>
    internal static class IntExtensions
    {
        /// <summary>
        ///     Check if current HTTP status code is success (OK | NoContent)
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns></returns>
        /// <remarks>Status200OK | Status204NoContent</remarks>
        internal static bool IsSuccessHttpStatus(this int statusCode)
            => statusCode == StatusCodes.Status200OK || statusCode == StatusCodes.Status204NoContent;

        /// <summary>
        ///     Check if current HTTP status code is success (NoContent)
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns></returns>
        /// <remarks>Status204NoContent</remarks>
        internal static bool IsSuccessHttpNoContentStatus(this int statusCode)
            => statusCode == StatusCodes.Status204NoContent;

        /// <summary>
        ///     Check if current HTTP status code is success (OK)
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns></returns>
        /// <remarks>Status200OK</remarks>
        internal static bool IsSuccessHttpOkStatus(this int statusCode)
            => statusCode == StatusCodes.Status200OK;
    }
}