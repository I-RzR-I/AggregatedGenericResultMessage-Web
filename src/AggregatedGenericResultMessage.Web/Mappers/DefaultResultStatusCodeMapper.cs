// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:21
// ***********************************************************************
//  <copyright file="DefaultResultStatusCodeMapper.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using System.Net;

#endregion

namespace RzR.ResultMessage.Web.Mappers
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Default mapper preserving the historical behavior:
    ///     <list type="bullet">
    ///         <item>Success with a response body -&gt; 200 OK.</item>
    ///         <item>Success without a body -&gt; 204 NoContent.</item>
    ///         <item>Failure -&gt; 400 BadRequest.</item>
    ///     </list>
    /// </summary>
    /// <seealso cref="T:RzR.ResultMessage.Web.Abstractions.IResultStatusCodeMapper"/>
    /// =================================================================================================
    public sealed class DefaultResultStatusCodeMapper : IResultStatusCodeMapper
    {
        /// <inheritdoc/>
        public HttpStatusCode Map(IResult result, bool hasResponseBody)
        {
            if (result.IsNull()|| result.IsSuccess.IsFalse())
                return HttpStatusCode.BadRequest;

            return hasResponseBody ? HttpStatusCode.OK : HttpStatusCode.NoContent;
        }
    }
}