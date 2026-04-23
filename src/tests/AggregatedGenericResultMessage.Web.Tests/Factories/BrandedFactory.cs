// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 21:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 21:27
// ***********************************************************************
//  <copyright file="BrandedFactory.cs" company="RzR SOFT & TECH">
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
    internal sealed class BrandedFactory : DefaultProblemDetailsResultFactory
    {
        protected override string ResolveType(ResultProblemDetailsContext context)
        {
            return $"https://errors.example.com/{(int)context.StatusCode}";
        }

        protected override string ResolveTitle(ResultProblemDetailsContext context)
        {
            return "Bad request occurred";
        }

        protected override string ResolveInstance(ResultProblemDetailsContext context)
        {
            return "/api/orders";
        }

        protected override void ApplyExtensions(ResultMessageProblemDetails problem,
            ResultProblemDetailsContext context)
        {
            base.ApplyExtensions(problem, context);
            problem.Extensions["traceId"] = "trace-123";
        }
    }
}