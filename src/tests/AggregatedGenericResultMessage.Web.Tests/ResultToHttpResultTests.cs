// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-23 08:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-23 12:26
// ***********************************************************************
//  <copyright file="ResultToHttpResultTests.cs" company="RzR SOFT & TECH">
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
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Extensions.MinimalApi;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToHttpResultTests
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
        public async Task ToHttpResult_Failure_DefaultFactory_Returns400ProblemJson()
        {
            using var host = await BuildHost(ep => ep.MapGet("/orders", () =>
            {
                var result = new Result { IsSuccess = false }.WithError("invalid");

                return result.ToHttpResult();
            }));

            var response = await host.GetTestClient().GetAsync("/orders");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);

            var problem = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Value<int>("status"));
            Assert.IsNotNull(problem["extensions"]?["ResultMessages"] ?? problem["ResultMessages"]);
        }

        [TestMethod]
        public async Task ToHttpResult_Generic_Success_Returns200WithBody()
        {
            using var host = await BuildHost(ep => ep.MapGet("/orders/1", () =>
            {
                var result = Result<int>.Success(42);

                return result.ToHttpResult();
            }));

            var response = await host.GetTestClient().GetAsync("/orders/1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("42", (await response.Content.ReadAsStringAsync()).Trim());
        }

        [TestMethod]
        public async Task ToHttpResult_PerCallOverrides_WinOverFactoryDefaults()
        {
            using var host = await BuildHost(ep => ep.MapGet("/orders/{id:int}", (int id) =>
            {
                var result = new Result { IsSuccess = false }.WithError("missing");
                return result.ToHttpResult(
                    HttpStatusCode.NotFound,
                    "Order not found",
                    "The requested order does not exist.",
                    $"/api/orders/{id}");
            }));

            var response = await host.GetTestClient().GetAsync("/orders/77");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            var problem = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.AreEqual("Order not found", problem.Value<string>("title"));
            Assert.AreEqual("The requested order does not exist.", problem.Value<string>("detail"));
            Assert.AreEqual("/api/orders/77", problem.Value<string>("instance"));
        }

        [TestMethod]
        public async Task ToHttpResult_CustomFactory_BrandsTheResponse()
        {
            ProblemDetailsResultFactory.Current = new StampingFactory();

            using var host = await BuildHost(ep => ep.MapGet("/x", () =>
            {
                var result = new Result { IsSuccess = false }.WithError("oops");

                return result.ToHttpResult();
            }));

            var problem = JObject.Parse(
                await (await host.GetTestClient().GetAsync("/x")).Content.ReadAsStringAsync());

            Assert.AreEqual("https://errors.test/400", problem.Value<string>("type"));
            Assert.AreEqual("stamped", problem["extensions"]?.Value<string>("stamp"));
        }

        [TestMethod]
        public async Task ResultMessageResults_From_MirrorsExtension()
        {
            using var host = await BuildHost(ep => ep.MapGet("/x", () =>
            {
                var result = Result<string>.Success("hello");

                return ResultMessageHttpResults.From(result);
            }));

            var response = await host.GetTestClient().GetAsync("/x");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            StringAssert.Contains(await response.Content.ReadAsStringAsync(), "hello");
        }

        private static Task<IHost> BuildHost(Action<IEndpointRouteBuilder> mapEndpoints)
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
    }
}