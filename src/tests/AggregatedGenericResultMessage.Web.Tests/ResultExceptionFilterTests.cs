// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 08:13
// ***********************************************************************
//  <copyright file="ResultExceptionFilterTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Filters;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultExceptionFilterTests
    {
        [TestInitialize]
        public void Reset()
        {
            ResultStatusCodeMapper.Current = new DefaultResultStatusCodeMapper();
            ProblemDetailsResultFactory.Current = new DefaultProblemDetailsResultFactory();
        }

        [TestCleanup]
        public void Restore()
        {
            ResultStatusCodeMapper.Current = new DefaultResultStatusCodeMapper();
            ProblemDetailsResultFactory.Current = new DefaultProblemDetailsResultFactory();
        }

        [TestMethod]
        public void NonResultException_IsIgnored()
        {
            var filter = new WebResultExceptionFilter();
            var context = NewExceptionContext(new InvalidOperationException("boom"));

            filter.OnException(context);

            Assert.IsNull(context.Result);
            Assert.IsFalse(context.ExceptionHandled);
        }

        [TestMethod]
        public void ResultException_WithoutStatusCode_UsesMapper()
        {
            var filter = new WebResultExceptionFilter();
            var result = new Result { IsSuccess = false }.WithError("bad");
            var context = NewExceptionContext(new WebResultException(result));

            filter.OnException(context);

            Assert.IsTrue(context.ExceptionHandled);
            var objectResult = (ObjectResult)context.Result!;
            Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var problem = (ResultMessageProblemDetails)objectResult.Value!;
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Status);
        }

        [TestMethod]
        public void ResultException_WithExplicitStatusCode_Wins()
        {
            var filter = new WebResultExceptionFilter();
            var result = new Result { IsSuccess = false }.WithError("missing");
            var context = NewExceptionContext(new WebResultException(result, HttpStatusCode.NotFound));

            filter.OnException(context);

            var objectResult = (ObjectResult)context.Result!;
            Assert.AreEqual(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [TestMethod]
        public void ResultException_PerCallOverrides_FlowToProblemDetails()
        {
            var filter = new WebResultExceptionFilter();
            var result = new Result { IsSuccess = false }.WithError("conflict");
            var context = NewExceptionContext(new WebResultException(
                result,
                HttpStatusCode.Conflict,
                "Already exists",
                "see body",
                "/api/orders/42"));

            filter.OnException(context);

            var problem = (ResultMessageProblemDetails)((ObjectResult)context.Result!).Value!;
            Assert.AreEqual("Already exists", problem.Title);
            Assert.AreEqual("see body", problem.Detail);
            Assert.AreEqual("/api/orders/42", problem.Instance);
        }

        [TestMethod]
        public void ResultException_FillsInstanceFromHttpContextWhenMissing()
        {
            var filter = new WebResultExceptionFilter();
            var result = new Result { IsSuccess = false }.WithError("oops");
            var context = NewExceptionContext(
                new WebResultException(result),
                "/api/branded/things");

            filter.OnException(context);

            var problem = (ResultMessageProblemDetails)((ObjectResult)context.Result!).Value!;
            Assert.AreEqual("/api/branded/things", problem.Instance);
        }

        [TestMethod]
        public void ResultException_GoesThroughCustomFactory()
        {
            ProblemDetailsResultFactory.Current = new StampingFactory();
            var filter = new WebResultExceptionFilter();
            var result = new Result { IsSuccess = false }.WithError("oops");
            var context = NewExceptionContext(new WebResultException(result));

            filter.OnException(context);

            var problem = (ResultMessageProblemDetails)((ObjectResult)context.Result!).Value!;
            Assert.AreEqual("https://errors.test/400", problem.Type);
            Assert.AreEqual("stamped", problem.Extensions["stamp"]);
        }

        private static ExceptionContext NewExceptionContext(Exception exception, string requestPath = null)
        {
            var httpContext = new DefaultHttpContext();
            if (requestPath != null)
                httpContext.Request.Path = requestPath;

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }
    }
}