// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 22:42
// ***********************************************************************
//  <copyright file="StampingFactory.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Web.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests.Factories
{
    internal sealed class StampingFactory : DefaultProblemDetailsResultFactory
    {
        protected override string ResolveType(ResultProblemDetailsContext context)
        {
            return $"https://errors.test/{(int)context.StatusCode}";
        }

        protected override void ApplyExtensions(ResultMessageProblemDetails problem,
            ResultProblemDetailsContext context)
        {
            base.ApplyExtensions(problem, context);
            problem.Extensions["stamp"] = "stamped";
        }
    }
}