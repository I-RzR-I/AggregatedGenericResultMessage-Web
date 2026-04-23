// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:32
// ***********************************************************************
//  <copyright file="ConflictOnDuplicateMapper.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using RzR.ResultMessage.Web.Abstractions;

#endregion

namespace RzR.ResultMessage.Web.Tests.Mappers
{
    internal sealed class ConflictOnDuplicateMapper : IResultStatusCodeMapper
    {
        public HttpStatusCode Map(IResult result, bool hasResponseBody)
        {
            if (result.IsSuccess)
                return hasResponseBody ? HttpStatusCode.OK : HttpStatusCode.NoContent;

            if (result.Messages.Any(m =>
                    m.Message?.Info?.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0))
                return HttpStatusCode.Conflict;

            return HttpStatusCode.BadRequest;
        }
    }
}