// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.TestWebApiNet8
//  Author           : RzR
//  Created On       : 2026-04-23 12:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 12:53
// ***********************************************************************
//  <copyright file="BrandedProblemDetailsResultFactory.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using Microsoft.AspNetCore.Http;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Models;

#endregion

namespace TestWebApiNet8.ProblemDetails
{
    public sealed class BrandedProblemDetailsResultFactory : DefaultProblemDetailsResultFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrandedProblemDetailsResultFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override string ResolveType(ResultProblemDetailsContext context)
        {
            return $"https://errors.testwebapinet8.local/{(int)context.StatusCode}";
        }

        protected override string ResolveInstance(ResultProblemDetailsContext context)
        {
            return _httpContextAccessor.HttpContext?.Request?.Path.Value;
        }

        protected override void ApplyExtensions(
            ResultMessageProblemDetails problem,
            ResultProblemDetailsContext context)
        {
            base.ApplyExtensions(problem, context);

            var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier
                          ?? Guid.NewGuid().ToString("N");

            problem.Extensions["traceId"] = traceId;
            problem.Extensions["service"] = "TestWebApiNet8";
        }
    }
}