// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:32
// ***********************************************************************
//  <copyright file="NotFoundOnMissingMapper.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Web.Abstractions;

#endregion

namespace RzR.ResultMessage.Web.Tests.Mappers
{
    internal sealed class NotFoundOnMissingMapper : IResultStatusCodeMapper
    {
        public HttpStatusCode Map(IResult result, bool hasResponseBody)
        {
            if (result.IsSuccess)
                return hasResponseBody ? HttpStatusCode.OK : HttpStatusCode.NoContent;

            if (result.Messages.Any(m =>
                    m.Message?.Info?.IndexOf("missing", StringComparison.OrdinalIgnoreCase) >= 0))
                return HttpStatusCode.NotFound;

            return HttpStatusCode.BadRequest;
        }
    }
}