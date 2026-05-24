// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 22:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 22:43
// ***********************************************************************
//  <copyright file="ResultExceptionMiddlewareTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Factories;
using RzR.ResultMessage.Web.WebDependencyInjection;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultExceptionMiddlewareTests
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
        public async Task NonResultException_RendersAs500ProblemDetailsByDefault()
        {
            using var host = await BuildHost(ctx => throw new InvalidOperationException("boom"));
            var client = host.GetTestClient();

            var response = await client.GetAsync("/oops");
            var body = await response.Content.ReadAsStringAsync();
            var problem = JObject.Parse(body);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, problem.Value<int>("status"));
            Assert.AreEqual("Unhandled exception", problem.Value<string>("title"));

            // Exception message is NOT exposed by default.
            StringAssert.DoesNotMatch(problem.Value<string>("detail") ?? string.Empty, new System.Text.RegularExpressions.Regex("boom"));
            Assert.AreEqual("/oops", problem.Value<string>("instance"));
        }

        [TestMethod]
        public async Task NonResultException_IncludeMessageOption_ExposesExceptionMessageInDetail()
        {
            using var host = await BuildHost(
                ctx => throw new InvalidOperationException("boom-from-test"),
                configure: o => o.IncludeExceptionMessageInDetail = true);
            var client = host.GetTestClient();

            var problem = JObject.Parse(await (await client.GetAsync("/x")).Content.ReadAsStringAsync());

            Assert.AreEqual("boom-from-test", problem.Value<string>("detail"));
        }

        [TestMethod]
        public async Task NonResultException_IncludeDetailsOption_AddsExceptionExtensions()
        {
            using var host = await BuildHost(
                ctx => throw new InvalidOperationException("boom"),
                configure: o => o.IncludeExceptionDetailsInExtensions = true);
            var client = host.GetTestClient();

            var problem = JObject.Parse(await (await client.GetAsync("/x")).Content.ReadAsStringAsync());

            var exception = problem["extensions"]?.Value<string>("exception") ?? problem.Value<string>("exception");
            var exceptionType = problem["extensions"]?.Value<string>("exceptionType") ?? problem.Value<string>("exceptionType");

            StringAssert.Contains(exception, "InvalidOperationException");
            Assert.AreEqual(typeof(InvalidOperationException).FullName, exceptionType);
        }

        [TestMethod]
        public async Task NonResultException_CustomStatusCodeOption_IsHonored()
        {
            using var host = await BuildHost(
                ctx => throw new InvalidOperationException("boom"),
                configure: o => o.DefaultUnhandledStatusCode = HttpStatusCode.BadGateway);
            var client = host.GetTestClient();

            var response = await client.GetAsync("/x");

            Assert.AreEqual(HttpStatusCode.BadGateway, response.StatusCode);
        }

        [TestMethod]
        public async Task OnException_CallbackIsInvokedForBothFlows()
        {
            var captured = new List<Exception>();

            using var host = await BuildHost(
                ctx => throw new InvalidOperationException("boom"),
                configure: o => o.OnException = (ex, _) => captured.Add(ex));
            var client = host.GetTestClient();

            await client.GetAsync("/x");

            Assert.AreEqual(1, captured.Count);
            Assert.IsInstanceOfType(captured[0], typeof(InvalidOperationException));
        }

        [TestMethod]
        public async Task ResultException_WithoutStatusCode_RendersMappedProblemDetails()
        {
            using var host = await BuildHost(ctx =>
            {
                var result = new Result { IsSuccess = false }.WithError("bad");
                throw new WebResultException(result);
            });
            var client = host.GetTestClient();

            var response = await client.GetAsync("/some/path");
            var body = await response.Content.ReadAsStringAsync();
            var problem = JObject.Parse(body);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Value<int>("status"));
            Assert.AreEqual("/some/path", problem.Value<string>("instance"));
            Assert.IsNotNull(problem["extensions"]?["ResultMessages"]);
        }

        [TestMethod]
        public async Task ResultException_WithExplicitStatus_AndOverrides_FlowThrough()
        {
            using var host = await BuildHost(ctx =>
            {
                var result = new Result { IsSuccess = false }.WithError("conflict");
                throw new WebResultException(
                    result,
                    HttpStatusCode.Conflict,
                    "Already exists",
                    "see body",
                    "/api/orders/42");
            });
            var client = host.GetTestClient();

            var response = await client.GetAsync("/ignored");
            var problem = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
            Assert.AreEqual("Already exists", problem.Value<string>("title"));
            Assert.AreEqual("see body", problem.Value<string>("detail"));
            Assert.AreEqual("/api/orders/42", problem.Value<string>("instance"));
        }

        [TestMethod]
        public async Task ResultException_RoutedThroughCustomFactory()
        {
            ProblemDetailsResultFactory.Current = new StampingFactory();

            using var host = await BuildHost(ctx =>
            {
                var result = new Result { IsSuccess = false }.WithError("oops");
                throw new WebResultException(result);
            });
            var client = host.GetTestClient();

            var problem = JObject.Parse(await (await client.GetAsync("/x")).Content.ReadAsStringAsync());

            Assert.AreEqual("https://errors.test/400", problem.Value<string>("type"));
            Assert.AreEqual("stamped", problem["extensions"]?.Value<string>("stamp"));
        }

        [TestMethod]
        public async Task ResultException_AutoPopulates_TraceId_FromHttpContext()
        {
            using var host = await BuildHost(ctx =>
            {
                ctx.TraceIdentifier = "trace-abc-123";
                var result = new Result { IsSuccess = false }.WithError("nope");
                throw new WebResultException(result);
            });
            var client = host.GetTestClient();

            var problem = JObject.Parse(await (await client.GetAsync("/x")).Content.ReadAsStringAsync());

            Assert.AreEqual("trace-abc-123", problem["extensions"]?.Value<string>("traceId"));
        }

        [TestMethod]
        public async Task UnhandledException_AutoPopulates_TraceId_FromHttpContext()
        {
            using var host = await BuildHost(ctx =>
            {
                ctx.TraceIdentifier = "trace-xyz-999";
                throw new InvalidOperationException("boom");
            });
            var client = host.GetTestClient();

            var problem = JObject.Parse(await (await client.GetAsync("/x")).Content.ReadAsStringAsync());

            Assert.AreEqual("trace-xyz-999", problem["extensions"]?.Value<string>("traceId"));
        }

        [TestMethod]
        public async Task TraceId_NotOverridden_When_CallerSuppliesItInAdditionalInformation()
        {
            using var host = await BuildHost(ctx =>
            {
                ctx.TraceIdentifier = "ambient-trace";
                var result = new Result { IsSuccess = false }.WithError("nope");
                throw new WebResultException(
                    result,
                    additionalInformation: new Dictionary<string, object> { ["traceId"] = "caller-trace" });
            });
            var client = host.GetTestClient();

            var problem = JObject.Parse(await (await client.GetAsync("/x")).Content.ReadAsStringAsync());

            Assert.AreEqual("caller-trace", problem["extensions"]?.Value<string>("traceId"));
        }

        private static Task<IHost> BuildHost(
            Action<HttpContext> terminalMiddleware,
            Action<WebResultExceptionMiddlewareOptions> configure = null)
        {
            return new HostBuilder()
                .ConfigureWebHost(web =>
                {
                    web.UseTestServer();
                    web.ConfigureServices(services =>
                    {
                        services.AddControllers();
                        services.AddResultExceptionMiddleware(configure);
                    });
                    web.Configure(app =>
                    {
                        app.UseResultExceptionMiddleware();
                        app.Run(ctx =>
                        {
                            terminalMiddleware(ctx);
                            return Task.CompletedTask;
                        });
                    });
                })
                .StartAsync();
        }
    }
}