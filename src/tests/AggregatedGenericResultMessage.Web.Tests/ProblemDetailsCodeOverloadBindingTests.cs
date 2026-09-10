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
using RzR.ResultMessage.Web.Tests.Factories;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeOverloadBindingTests
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
        public void HttpResultsFrom_WithAConcreteResultReceiver_BindsTheGenericOverload_Test()
        {
            var recorder = new OverloadRecordingFactory();
            ProblemDetailsResultFactory.Current = recorder;

            var sut = ProblemDetailsCodeFixture.MultiMessageFailure();
            ResultMessageHttpResults.From(sut);

            Assert.AreEqual(1, recorder.CreateCallCount, "Exactly one factory invocation is expected.");
            Assert.IsTrue(recorder.RecordedHasResponseBody.HasValue, "The factory must have been invoked.");
            Assert.IsTrue(
                recorder.RecordedHasResponseBody.Value,
                "Result derives from Result<object>, so IResult<object> is a strictly better conversion "
                + "target than IResult and the generic From<T> overload wins. HasResponseBody is set only "
                + "by the generic path, so it proves the binding at runtime rather than by inspection.");
        }

        [TestMethod]
        public void HttpResultsFrom_WithAnIResultTypedReceiver_BindsTheNonGenericOverload_Test()
        {
            var recorder = new OverloadRecordingFactory();
            ProblemDetailsResultFactory.Current = recorder;

            IResult sut = ProblemDetailsCodeFixture.MultiMessageFailure();
            ResultMessageHttpResults.From(sut);

            Assert.IsTrue(recorder.RecordedHasResponseBody.HasValue, "The factory must have been invoked.");
            Assert.IsFalse(
                recorder.RecordedHasResponseBody.Value,
                "Widening the receiver to IResult is the only way to reach the non-generic From overload, "
                + "so this is the call shape that gives that overload genuine coverage.");
        }

        [TestMethod]
        public void ToHttpResult_WithAConcreteResultReceiver_BindsTheGenericOverload_Test()
        {
            var recorder = new OverloadRecordingFactory();
            ProblemDetailsResultFactory.Current = recorder;

            var sut = ProblemDetailsCodeFixture.MultiMessageFailure();
            sut.ToHttpResult();

            Assert.IsTrue(recorder.RecordedHasResponseBody.HasValue, "The factory must have been invoked.");
            Assert.IsTrue(recorder.RecordedHasResponseBody.Value);
        }

        [TestMethod]
        public void ToHttpResult_WithAnIResultTypedReceiver_BindsTheNonGenericOverload_Test()
        {
            var recorder = new OverloadRecordingFactory();
            ProblemDetailsResultFactory.Current = recorder;

            IResult sut = ProblemDetailsCodeFixture.MultiMessageFailure();
            sut.ToHttpResult();

            Assert.IsTrue(recorder.RecordedHasResponseBody.HasValue, "The factory must have been invoked.");
            Assert.IsFalse(recorder.RecordedHasResponseBody.Value);
        }

        [TestMethod]
        public async Task HttpResultsFrom_BothOverloads_EmitTheIdenticalCodeTitleAndStatus_Test()
        {
            using var host = await BuildHost();
            var client = host.GetTestClient();

            var generic = JObject.Parse(await (await client.GetAsync("/from-generic")).Content.ReadAsStringAsync());
            var nonGeneric = JObject.Parse(
                await (await client.GetAsync("/from-non-generic")).Content.ReadAsStringAsync());

            Assert.AreEqual(
                ProblemDetailsCodeFixture.FirstKey,
                ProblemDetailsCodeFixture.ReadCode(generic));
            Assert.AreEqual(
                ProblemDetailsCodeFixture.ReadCode(generic),
                ProblemDetailsCodeFixture.ReadCode(nonGeneric),
                "The two From overloads differ only in the success-path response body, so a failing "
                + "result must render an identical code on both.");
            Assert.AreEqual(generic.Value<string>("title"), nonGeneric.Value<string>("title"));
            Assert.AreEqual(generic.Value<int>("status"), nonGeneric.Value<int>("status"));
        }

        [TestMethod]
        public async Task HttpResultsFrom_BothOverloads_OmitTheCodeMember_WhenNoMessageCarriesAKey_Test()
        {
            using var host = await BuildHost();
            var client = host.GetTestClient();

            foreach (var path in new[] { "/from-generic-unkeyed", "/from-non-generic-unkeyed" })
            {
                var raw = await (await client.GetAsync(path)).Content.ReadAsStringAsync();

                Assert.IsFalse(
                    ProblemDetailsCodeFixture.HasCodeMember(JObject.Parse(raw)),
                    "Path emitted a code member for a result with no keys: " + path + ". Body: "
                    + ProblemDetailsCodePairingFixture.Escape(raw));
            }
        }

        [TestMethod]
        public async Task ToHttpResult_NonGenericSuccessResult_Returns200_WhileToProblemResponse_Returns204_KnownDivergence_Test()
        {
            using var host = await BuildHost();
            var client = host.GetTestClient();

            var viaHttpResult = await client.GetAsync("/success-http-result");
            var viaProblemResponse = await client.GetAsync("/success-problem-response");

            Assert.AreEqual(
                HttpStatusCode.OK,
                viaHttpResult.StatusCode,
                "Known divergence: ResultToHttpResult has no 'this Result' overload, so a non-generic "
                + "Result binds ToHttpResult<object> and is mapped as if it carried a response body.");
            Assert.AreEqual(
                HttpStatusCode.NoContent,
                viaProblemResponse.StatusCode,
                "ToProblemResponse does declare a 'this Result' overload, so the same value yields 204 "
                + "there. The two unified surfaces disagree on the non-generic success path.");
        }

        private static Task<IHost> BuildHost()
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
                            endpoints.MapGet("/from-generic", () =>
                                ResultMessageHttpResults.From(ProblemDetailsCodeFixture.MultiMessageFailure()));

                            endpoints.MapGet("/from-non-generic", () =>
                            {
                                IResult typed = ProblemDetailsCodeFixture.MultiMessageFailure();

                                return ResultMessageHttpResults.From(typed);
                            });

                            endpoints.MapGet("/from-generic-unkeyed", () =>
                                ResultMessageHttpResults.From(ProblemDetailsCodeFixture.UnkeyedFailure()));

                            endpoints.MapGet("/from-non-generic-unkeyed", () =>
                            {
                                IResult typed = ProblemDetailsCodeFixture.UnkeyedFailure();

                                return ResultMessageHttpResults.From(typed);
                            });

                            endpoints.MapGet("/success-http-result", () =>
                                new Result { IsSuccess = true }.ToHttpResult());

                            endpoints.MapGet("/success-problem-response", () =>
                                new Result { IsSuccess = true }.ToProblemResponse());
                        });
                    });
                })
                .StartAsync();
        }
    }
}
