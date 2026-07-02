// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author            : RzR
//  Created           : 02-07-2026 19:07
// 
//  Last Modified By : RzR
//  Last Modified On : 02-07-2026 19:58
//  ***********************************************************************
//  <copyright file="ResultToProblemResponseEndToEndTests.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

#region U S I N G

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Extensions.Unified;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToProblemResponseEndToEndTests
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

        private static Task<IHost> BuildMinimalHost(Action<IEndpointRouteBuilder> mapEndpoints)
        {
            return new HostBuilder()
                .ConfigureWebHost(web =>
                {
                    web.UseTestServer();
                    web.ConfigureServices(services => services.AddRouting());
                    web.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => mapEndpoints(endpoints));
                    });
                })
                .StartAsync();
        }

        private static Task<IHost> BuildMvcHost()
        {
            return new HostBuilder()
                .ConfigureWebHost(web =>
                {
                    web.UseTestServer();
                    web.ConfigureServices(services =>
                    {
                        services.AddRouting();

                        services.AddControllers()
                            .AddApplicationPart(typeof(ProblemResponseTestController).Assembly);
                    });
                    web.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapControllers());
                    });
                })
                .StartAsync();
        }

        #region Minimal-API host: proves the pipeline dispatches via Http.IResult.ExecuteAsync

        [TestMethod]
        public async Task MinimalApi_GenericSuccess_Returns200_WithExactBody_NotSerializedEnvelope()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/ok", () =>
                Result<int>.Success(42).ToProblemResponse()));

            var response = await host.GetTestClient().GetAsync("/ok");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = (await response.Content.ReadAsStringAsync()).Trim();
            Assert.AreEqual("42", body);
        }

        [TestMethod]
        public async Task MinimalApi_Failure_DefaultFactory_Returns400_ProblemJson()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
            {
                var result = new Result { IsSuccess = false }.WithError("invalid");

                return result.ToProblemResponse();
            }));

            var response = await host.GetTestClient().GetAsync("/fail");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);

            var problem = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Value<int>("status"));
            Assert.IsNotNull(problem["extensions"]?["ResultMessages"] ?? problem["ResultMessages"]);
        }

        [TestMethod]
        public async Task MinimalApi_Failure_PerCallOverrides_Returns404_WithTitleDetailInstance()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/orders/{id:int}", (int id) =>
            {
                var result = new Result { IsSuccess = false }.WithError("missing");

                return result.ToProblemResponse(
                    HttpStatusCode.NotFound,
                    "Order not found",
                    "no order",
                    $"/api/orders/{id}");
            }));

            var response = await host.GetTestClient().GetAsync("/orders/7");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            var problem = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.AreEqual("Order not found", problem.Value<string>("title"));
            Assert.AreEqual("no order", problem.Value<string>("detail"));
            Assert.AreEqual("/api/orders/7", problem.Value<string>("instance"));
        }

        [TestMethod]
        public async Task MinimalApi_NonGenericSuccess_Returns204_NoBody()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/nocontent", () =>
                new Result { IsSuccess = true }.ToProblemResponse()));

            var response = await host.GetTestClient().GetAsync("/nocontent");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var body = (await response.Content.ReadAsStringAsync()).Trim();
            Assert.AreEqual(string.Empty, body);
        }

        #endregion

        #region MVC controllers host: proves the pipeline dispatches via IActionResult.ExecuteResultAsync

        [TestMethod]
        public async Task Mvc_GenericSuccess_Returns200_WithExactBody_ViaRealControllerDispatch()
        {
            using var host = await BuildMvcHost();

            var response = await host.GetTestClient().GetAsync("/api/problem-response/ok");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = (await response.Content.ReadAsStringAsync()).Trim();
            Assert.AreEqual("42", body);
        }

        [TestMethod]
        public async Task Mvc_Failure_Returns400_ProblemJson_ViaRealControllerDispatch()
        {
            using var host = await BuildMvcHost();

            var response = await host.GetTestClient().GetAsync("/api/problem-response/fail");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);

            var problem = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Value<int>("status"));
            Assert.IsNotNull(problem["extensions"]?["ResultMessages"] ?? problem["ResultMessages"]);
        }

        [TestMethod]
        public async Task Mvc_NonGenericSuccess_Returns204_NoBody_ViaRealControllerDispatch()
        {
            using var host = await BuildMvcHost();

            var response = await host.GetTestClient().GetAsync("/api/problem-response/nocontent");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var body = (await response.Content.ReadAsStringAsync()).Trim();
            Assert.IsTrue(body.Length == 0 || body == "null", $"Expected an empty or null body, got: '{body}'");
        }

        #endregion
    }

    [ApiController]
    [Route("api/problem-response")]
    public class ProblemResponseTestController : ControllerBase
    {
        [HttpGet("ok")]
        public IActionResult Ok42()
        {
            return Result<int>.Success(42).ToProblemResponse();
        }

        [HttpGet("fail")]
        public IActionResult Fail()
        {
            return new Result { IsSuccess = false }.WithError("invalid").ToProblemResponse();
        }

        [HttpGet("nocontent")]
        public IActionResult NoBody()
        {
            return new Result { IsSuccess = true }.ToProblemResponse();
        }
    }
}