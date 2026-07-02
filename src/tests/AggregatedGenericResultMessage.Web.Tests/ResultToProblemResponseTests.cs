// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author            : RzR
//  Created           : 02-07-2026 19:07
// 
//  Last Modified By : RzR
//  Last Modified On : 02-07-2026 19:59
//  ***********************************************************************
//  <copyright file="ResultToProblemResponseTests.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

#region U S I N G

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Extensions.Unified;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToProblemResponseTests
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
        public async Task ToProblemResponse_NonGenericSuccess_ExecuteResultAsync_Returns204()
        {
            var actionContext = NewActionContext(out var bodyStream);
            var sut = new Result { IsSuccess = true };

            var response = sut.ToProblemResponse();

            Assert.IsInstanceOfType(response, typeof(IActionResult));
            await response.ExecuteResultAsync(actionContext);

            Assert.AreEqual(StatusCodes.Status204NoContent, actionContext.HttpContext.Response.StatusCode);

            var body = (await ReadBodyAsync(bodyStream)).Trim();
            Assert.IsTrue(body.Length == 0 || body == "null");
        }

        [TestMethod]
        public async Task ToProblemResponse_GenericSuccess_ExecuteResultAsync_Returns200_WithBody()
        {
            var actionContext = NewActionContext(out var bodyStream);
            var sut = Result<int>.Success(42);

            var response = sut.ToProblemResponse();

            await response.ExecuteResultAsync(actionContext);

            Assert.AreEqual(StatusCodes.Status200OK, actionContext.HttpContext.Response.StatusCode);
            Assert.AreEqual("42", (await ReadBodyAsync(bodyStream)).Trim());
        }

        [TestMethod]
        public async Task ToProblemResponse_Failure_ExecuteResultAsync_Returns400_WithProblemDetailsBody()
        {
            var actionContext = NewActionContext(out var bodyStream);
            var sut = new Result { IsSuccess = false }.WithError("invalid");

            var response = sut.ToProblemResponse();

            await response.ExecuteResultAsync(actionContext);

            Assert.AreEqual(StatusCodes.Status400BadRequest, actionContext.HttpContext.Response.StatusCode);

            var problem = JObject.Parse(await ReadBodyAsync(bodyStream));
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Value<int>("status"));
            Assert.IsNotNull(problem["extensions"]?["ResultMessages"] ?? problem["ResultMessages"]);
        }

        [TestMethod]
        public void ToProblemResponse_Returns_DualInterfaceResponse()
        {
            var sut = new Result { IsSuccess = true };

            var response = sut.ToProblemResponse();

            Assert.IsInstanceOfType(response, typeof(IActionResult));
#if NET6_0_OR_GREATER
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Http.IResult));
#endif
        }

        [TestMethod]
        public async Task ToProblemResponse_ResultVsResultOfT_NonGenericIsNotHijackedByGenericOverload()
        {
            var nonGenericContext = NewActionContext(out _);
            var genericContext = NewActionContext(out var genericBody);

            var nonGenericResponse = new Result { IsSuccess = true }.ToProblemResponse();
            var genericResponse = Result<int>.Success(7).ToProblemResponse();

            await nonGenericResponse.ExecuteResultAsync(nonGenericContext);
            await genericResponse.ExecuteResultAsync(genericContext);

            Assert.AreEqual(StatusCodes.Status204NoContent, nonGenericContext.HttpContext.Response.StatusCode);
            Assert.AreEqual(StatusCodes.Status200OK, genericContext.HttpContext.Response.StatusCode);
            Assert.AreEqual("7", (await ReadBodyAsync(genericBody)).Trim());
        }

        [TestMethod]
        public async Task ToProblemResponse_WithHttpContext_PropagatesTraceId_OnFailure()
        {
            var actionContext = NewActionContext(out var bodyStream);
            actionContext.HttpContext.TraceIdentifier = "mvc-trace-1";
            var sut = new Result { IsSuccess = false }.WithError("nope");

            var response = sut.ToProblemResponse(httpContext: actionContext.HttpContext);

            await response.ExecuteResultAsync(actionContext);

            var problem = JObject.Parse(await ReadBodyAsync(bodyStream));
            var traceId = problem["extensions"]?.Value<string>("traceId") ?? problem.Value<string>("traceId");
            Assert.AreEqual("mvc-trace-1", traceId);
        }

        private static ActionContext NewActionContext(out MemoryStream bodyStream)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddControllers();

            bodyStream = new MemoryStream();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider(),
                Response = { Body = bodyStream }
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        private static async Task<string> ReadBodyAsync(MemoryStream bodyStream)
        {
            bodyStream.Position = 0;
            using var reader = new StreamReader(bodyStream, leaveOpen: true);

            return await reader.ReadToEndAsync();
        }

#if NET6_0_OR_GREATER
        [TestMethod]
        public async Task ToProblemResponse_Failure_ExecuteAsync_MinimalApiPath_Returns400_ProblemJson()
        {
            var httpContext = NewHttpContext(out var bodyStream);
            var sut = new Result { IsSuccess = false }.WithError("bad");

            var response = sut.ToProblemResponse();

            await response.ExecuteAsync(httpContext);

            Assert.AreEqual(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
            StringAssert.StartsWith(httpContext.Response.ContentType, "application/problem+json");

            var problem = JObject.Parse(await ReadBodyAsync(bodyStream));
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Value<int>("status"));
        }

        [TestMethod]
        public async Task ToProblemResponse_GenericSuccess_ExecuteAsync_MinimalApiPath_Returns200_WithBody()
        {
            var httpContext = NewHttpContext(out var bodyStream);
            var sut = Result<int>.Success(42);

            var response = sut.ToProblemResponse();

            await response.ExecuteAsync(httpContext);

            Assert.AreEqual(StatusCodes.Status200OK, httpContext.Response.StatusCode);
            Assert.AreEqual("42", (await ReadBodyAsync(bodyStream)).Trim());
        }

        private static HttpContext NewHttpContext(out MemoryStream bodyStream)
        {
            var services = new ServiceCollection();
            services.AddLogging();

            bodyStream = new MemoryStream();

            return new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider(),
                Response = { Body = bodyStream }
            };
        }
#endif
    }
}