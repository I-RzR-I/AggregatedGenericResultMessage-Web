#region U S I N G

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Extensions.MinimalApi;
using RzR.ResultMessage.Web.Extensions.Unified;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Controllers;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeParityTests
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
        public async Task ToProblemResponse_MvcAndMinimalApiDispatch_EmitTheSameCode_Test()
        {
            using var mvcHost = await BuildMvcHost();
            using var minimalHost = await BuildMinimalHost();

            var mvcCode = await ReadCodeAsync(mvcHost, "/api/code-parity/to-problem-response");
            var minimalCode = await ReadCodeAsync(minimalHost, "/to-problem-response");

            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, mvcCode);
            Assert.AreEqual(
                mvcCode,
                minimalCode,
                "The same ToProblemResponse() value must render the same code on both dispatch paths.");
        }

        [TestMethod]
        public async Task AsProblemDetails_Mvc_And_ToHttpResult_MinimalApi_EmitTheSameCode_Test()
        {
            using var mvcHost = await BuildMvcHost();
            using var minimalHost = await BuildMinimalHost();

            var mvcCode = await ReadCodeAsync(mvcHost, "/api/code-parity/as-problem-details");
            var minimalCode = await ReadCodeAsync(minimalHost, "/to-http-result");

            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, mvcCode);
            Assert.AreEqual(mvcCode, minimalCode);
        }

        [TestMethod]
        public async Task ResultMessageHttpResultsFrom_MirrorsToHttpResult_ForTheCodeMember_Test()
        {
            using var minimalHost = await BuildMinimalHost();

            var viaExtension = await ReadCodeAsync(minimalHost, "/to-http-result");
            var viaFactoryMethod = await ReadCodeAsync(minimalHost, "/http-results-from");

            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, viaFactoryMethod);
            Assert.AreEqual(viaExtension, viaFactoryMethod);
        }

        [TestMethod]
        public async Task AllFourPublicSurfaces_EmitTheIdenticalCode_ForTheSameFailingResult_Test()
        {
            using var mvcHost = await BuildMvcHost();
            using var minimalHost = await BuildMinimalHost();

            var codes = new Dictionary<string, string>
            {
                ["mvc:AsProblemDetails"] = await ReadCodeAsync(mvcHost, "/api/code-parity/as-problem-details"),
                ["mvc:ToProblemResponse"] = await ReadCodeAsync(mvcHost, "/api/code-parity/to-problem-response"),
                ["minimal:ToProblemResponse"] = await ReadCodeAsync(minimalHost, "/to-problem-response"),
                ["minimal:ToHttpResult"] = await ReadCodeAsync(minimalHost, "/to-http-result"),
                ["minimal:ResultMessageHttpResults.From"] = await ReadCodeAsync(minimalHost, "/http-results-from")
            };

            var disagreeing = codes
                .Where(entry => entry.Value != ProblemDetailsCodeFixture.FirstKey)
                .Select(entry => $"{entry.Key} => '{entry.Value ?? "<absent>"}'")
                .ToList();

            Assert.AreEqual(
                0,
                disagreeing.Count,
                $"Every surface must emit '{ProblemDetailsCodeFixture.FirstKey}'. Disagreeing: "
                + string.Join("; ", disagreeing));
        }

        [TestMethod]
        public async Task AllFourPublicSurfaces_OmitTheCodeMember_WhenNoMessageCarriesAKey_Test()
        {
            using var mvcHost = await BuildMvcHost();
            using var minimalHost = await BuildMinimalHost();

            var surfaces = new List<Tuple<IHost, string>>
            {
                Tuple.Create(mvcHost, "/api/code-parity/as-problem-details-unkeyed"),
                Tuple.Create(mvcHost, "/api/code-parity/to-problem-response-unkeyed"),
                Tuple.Create(minimalHost, "/to-problem-response-unkeyed"),
                Tuple.Create(minimalHost, "/to-http-result-unkeyed"),
                Tuple.Create(minimalHost, "/http-results-from-unkeyed")
            };

            foreach (var surface in surfaces)
            {
                var raw = await (await surface.Item1.GetTestClient().GetAsync(surface.Item2))
                    .Content.ReadAsStringAsync();
                var problem = JObject.Parse(raw);

                Assert.IsFalse(
                    ProblemDetailsCodeFixture.HasCodeMember(problem),
                    $"'{surface.Item2}' emitted a 'code' member for a result with no keys. Body: {raw}");
            }
        }

        [TestMethod]
        public async Task ToProblemResponse_MvcAndMinimalApiDispatch_AgreeOnStatusTitleDetailAndCodeTogether_Test()
        {
            using var mvcHost = await BuildMvcHost();
            using var minimalHost = await BuildMinimalHost();

            var mvc = JObject.Parse(await (await mvcHost.GetTestClient()
                .GetAsync("/api/code-parity/to-problem-response")).Content.ReadAsStringAsync());
            var minimal = JObject.Parse(await (await minimalHost.GetTestClient()
                .GetAsync("/to-problem-response")).Content.ReadAsStringAsync());

            Assert.AreEqual(mvc.Value<int>("status"), minimal.Value<int>("status"));
            Assert.AreEqual(mvc.Value<string>("title"), minimal.Value<string>("title"));
            Assert.AreEqual(mvc.Value<string>("detail"), minimal.Value<string>("detail"));
            Assert.AreEqual(mvc.Value<string>("code"), minimal.Value<string>("code"));
            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, mvc.Value<string>("code"));
        }

        private static async Task<string> ReadCodeAsync(IHost host, string path)
        {
            var raw = await (await host.GetTestClient().GetAsync(path)).Content.ReadAsStringAsync();

            return ProblemDetailsCodeFixture.ReadCode(JObject.Parse(raw));
        }

        private static Task<IHost> BuildMinimalHost()
        {
            return new HostBuilder()
                .ConfigureWebHost(web =>
                {
                    web.UseTestServer();
                    web.ConfigureServices(services => services.AddRouting());
                    web.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/to-problem-response", () =>
                                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse());

                            endpoints.MapGet("/to-http-result", () =>
                                ProblemDetailsCodeFixture.MultiMessageFailure().ToHttpResult());

                            endpoints.MapGet("/http-results-from", () =>
                                ResultMessageHttpResults.From(ProblemDetailsCodeFixture.MultiMessageFailure()));

                            endpoints.MapGet("/to-problem-response-unkeyed", () =>
                                ProblemDetailsCodeFixture.UnkeyedFailure().ToProblemResponse());

                            endpoints.MapGet("/to-http-result-unkeyed", () =>
                                ProblemDetailsCodeFixture.UnkeyedFailure().ToHttpResult());

                            endpoints.MapGet("/http-results-from-unkeyed", () =>
                                ResultMessageHttpResults.From(ProblemDetailsCodeFixture.UnkeyedFailure()));
                        });
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
                            .AddApplicationPart(typeof(ProblemDetailsCodeParityController).Assembly);
                    });
                    web.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapControllers());
                    });
                })
                .StartAsync();
        }
    }
}
