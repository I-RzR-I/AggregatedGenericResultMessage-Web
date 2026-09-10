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
using RzR.ResultMessage.Web.Tests.Controllers;
using RzR.ResultMessage.Web.Tests.Factories;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeFunctionalTests
    {
        private const string OverriddenCode = "c-GLOBAL-OVERRIDE";

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
        public async Task MinimalApi_FailureWithKeyedFirstMessage_SerializedBodyCarriesTopLevelCode_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse()));

            var response = await host.GetTestClient().GetAsync("/fail");
            var raw = await response.Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                $"The serialized body carries no top-level 'code' member. Body: {raw}");
            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, ProblemDetailsCodeFixture.ReadCode(problem));
            Assert.IsNull(
                problem["extensions"]?["code"],
                "'code' must be a top-level member, not an extension entry.");
        }

        [TestMethod]
        public async Task Mvc_FailureWithKeyedFirstMessage_SerializedBodyCarriesTopLevelCode_Test()
        {
            using var host = await BuildMvcHost();

            var response = await host.GetTestClient().GetAsync("/api/code-functional/multi-message-fail");
            var raw = await response.Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                $"The serialized body carries no top-level 'code' member. Body: {raw}");
            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, ProblemDetailsCodeFixture.ReadCode(problem));
            Assert.IsNull(
                problem["extensions"]?["code"],
                "'code' must be a top-level member, not an extension entry.");
        }

        [TestMethod]
        public async Task MinimalApi_FailureWithNoKeyOnAnyMessage_OmitsCodeMemberEntirely_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.UnkeyedFailure().ToProblemResponse()));

            var raw = await (await host.GetTestClient().GetAsync("/fail")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                $"'code' must be omitted entirely - not emitted as null or \"\". Body: {raw}");
        }

        [TestMethod]
        public async Task Mvc_FailureWithNoKeyOnAnyMessage_OmitsCodeMemberEntirely_Test()
        {
            using var host = await BuildMvcHost();

            var raw = await (await host.GetTestClient().GetAsync("/api/code-functional/unkeyed-fail"))
                .Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                $"'code' must be omitted entirely - not emitted as null or \"\". Body: {raw}");
        }

        [TestMethod]
        public async Task MinimalApi_MultipleKeyedMessages_CodeIsTheKeyOfTheMessageSupplyingTitleAndDetail_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse()));

            var problem = JObject.Parse(
                await (await host.GetTestClient().GetAsync("/fail")).Content.ReadAsStringAsync());

            var code = ProblemDetailsCodeFixture.ReadCode(problem);

            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, code);
            Assert.AreNotEqual(
                ProblemDetailsCodeFixture.SecondKey,
                code,
                "code must be the key of the message that supplies title and detail; a later message's key "
                + "must never be substituted for it.");
        }

        [TestMethod]
        public async Task MinimalApi_MultipleKeyedMessages_SelectionIsRepeatableAcrossRequests_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse()));

            var client = host.GetTestClient();
            var observed = new List<string>();

            for (var attempt = 0; attempt < 5; attempt++)
            {
                var problem = JObject.Parse(await (await client.GetAsync("/fail")).Content.ReadAsStringAsync());
                observed.Add(ProblemDetailsCodeFixture.ReadCode(problem));
            }

            CollectionAssert.AreEqual(
                Enumerable.Repeat(ProblemDetailsCodeFixture.FirstKey, 5).ToList(),
                observed,
                $"Code selection must be deterministic. Observed: {string.Join(", ", observed)}");
        }

        [TestMethod]
        public async Task MinimalApi_FirstMessageUnkeyed_SecondKeyed_SerializedCodeIsOmitted_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.LeadingUnkeyedFailure().ToProblemResponse()));

            var raw = await (await host.GetTestClient().GetAsync("/fail")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "The message supplying title and detail is unkeyed, so 'code' must be omitted rather "
                + $"than taken from the later keyed message. Body: {raw}");
            CollectionAssert.Contains(
                ProblemDetailsCodeFixture.ReadResultMessageKeys(problem).ToList(),
                ProblemDetailsCodeFixture.LateKey,
                $"The later key must stay visible under extensions.ResultMessages. Body: {raw}");
        }

        [TestMethod]
        public async Task Mvc_FirstMessageUnkeyed_SecondKeyed_SerializedCodeIsOmitted_Test()
        {
            using var host = await BuildMvcHost();

            var raw = await (await host.GetTestClient().GetAsync("/api/code-functional/leading-unkeyed-fail"))
                .Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "The message supplying title and detail is unkeyed, so 'code' must be omitted rather "
                + $"than taken from the later keyed message. Body: {raw}");
            CollectionAssert.Contains(
                ProblemDetailsCodeFixture.ReadResultMessageKeys(problem).ToList(),
                ProblemDetailsCodeFixture.LateKey,
                $"The later key must stay visible under extensions.ResultMessages. Body: {raw}");
        }

        [TestMethod]
        public void Factory_FirstMessageUnkeyed_SecondKeyed_ResolvesNoCode_PropertyLevelOnly_Test()
        {
            var sut = ProblemDetailsCodeFixture.LeadingUnkeyedFailure();

            var result = sut.AsProblemDetails(HttpStatusCode.BadRequest);

            var problem = (ResultMessageProblemDetails)result.Value!;

            Assert.IsNull(
                problem.Code,
                "The message supplying Title and Detail is unkeyed, so Code must stay null rather than "
                + $"take the later message's key. Title was: '{problem.Title}'");
            Assert.AreEqual(
                sut.Messages.Last().Key,
                ProblemDetailsCodeFixture.LateKey,
                "Test data guard: the second message really does carry the later key.");
        }

        [TestMethod]
        public async Task MinimalApi_NonGenericSuccess_Returns204_WithByteIdenticalEmptyBody_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/ok", () =>
                new Result { IsSuccess = true }.ToProblemResponse()));

            var response = await host.GetTestClient().GetAsync("/ok");
            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.AreEqual(string.Empty, body.Trim());
        }

        [TestMethod]
        public async Task MinimalApi_GenericSuccess_Returns200_WithByteIdenticalScalarBody_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/ok", () =>
                Result<int>.Success(42).ToProblemResponse()));

            var response = await host.GetTestClient().GetAsync("/ok");
            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual("42", body.Trim());
        }

        [TestMethod]
        public async Task Mvc_GenericSuccess_Returns200_WithByteIdenticalScalarBody_Test()
        {
            using var host = await BuildMvcHost();

            var response = await host.GetTestClient().GetAsync("/api/code-functional/generic-success");
            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("42", body.Trim());
        }

        [TestMethod]
        public async Task Mvc_NonGenericSuccess_Returns204_WithNoProblemBody_Test()
        {
            using var host = await BuildMvcHost();

            var response = await host.GetTestClient().GetAsync("/api/code-functional/non-generic-success");
            var body = (await response.Content.ReadAsStringAsync()).Trim();

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.IsTrue(body.Length == 0 || body == "null", $"Expected an empty or null body, got: '{body}'");
        }

        [TestMethod]
        public void Factory_GenericSuccess_ProducesNoProblemDetailsInstanceAtAll_Test()
        {
            var sut = Result<int>.Success(42);

            var result = sut.AsProblemDetails(HttpStatusCode.OK);

            Assert.IsNotInstanceOfType(result.Value, typeof(ResultMessageProblemDetails));
            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        public async Task MinimalApi_PerCallTitleDetailInstanceOverrides_DoNotAlterTheEmittedCode_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse(
                    HttpStatusCode.NotFound,
                    "Order not found",
                    "no order",
                    "/api/orders/7")));

            var raw = await (await host.GetTestClient().GetAsync("/fail")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual("Order not found", problem.Value<string>("title"));
            Assert.AreEqual("no order", problem.Value<string>("detail"));
            Assert.AreEqual("/api/orders/7", problem.Value<string>("instance"));
            Assert.AreEqual(
                ProblemDetailsCodeFixture.FirstKey,
                ProblemDetailsCodeFixture.ReadCode(problem),
                $"Per-call title/detail/instance overrides must not bleed into 'code'. Body: {raw}");
        }

        [TestMethod]
        public async Task MinimalApi_CustomFactoryOverridingResolveCode_ItsValueReachesTheWire_Test()
        {
            ProblemDetailsResultFactory.Current = new HostileCodeFactory { CodeToReturn = OverriddenCode };

            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse()));

            var raw = await (await host.GetTestClient().GetAsync("/fail")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(
                OverriddenCode,
                ProblemDetailsCodeFixture.ReadCode(problem),
                $"ResolveCode is the single selection point for 'code'. Body: {raw}");
        }

        [TestMethod]
        public async Task MinimalApi_Failure_StillCarriesEveryMessageWithItsOwnKeyUnderExtensions_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse()));

            var raw = await (await host.GetTestClient().GetAsync("/fail")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            var keys = ProblemDetailsCodeFixture.ReadResultMessageKeys(problem);
            var messages = (JArray)problem["extensions"]!["ResultMessages"]!;

            Assert.AreEqual(3, keys.Count, $"All three messages must survive in extensions. Body: {raw}");
            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, keys[0]);
            Assert.AreEqual(ProblemDetailsCodeFixture.SecondKey, keys[1]);
            Assert.IsTrue(
                string.IsNullOrWhiteSpace(keys[2]),
                $"The third message was authored unkeyed and must stay unkeyed, got: '{keys[2]}'");

            foreach (var info in new[] { "First validation error", "Second validation error", "Third validation error" })
            {
                StringAssert.Contains(
                    messages.ToString(),
                    info,
                    $"Message payloads must be unchanged by the new 'code' member. Body: {raw}");
            }
        }

        [TestMethod]
        public async Task Mvc_Failure_StillCarriesEveryMessageWithItsOwnKeyUnderExtensions_Test()
        {
            using var host = await BuildMvcHost();

            var raw = await (await host.GetTestClient().GetAsync("/api/code-functional/multi-message-fail"))
                .Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            var keys = ProblemDetailsCodeFixture.ReadResultMessageKeys(problem);

            Assert.AreEqual(3, keys.Count, $"All three messages must survive in extensions. Body: {raw}");
            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, keys[0]);
            Assert.AreEqual(ProblemDetailsCodeFixture.SecondKey, keys[1]);
        }

        [TestMethod]
        public async Task MinimalApi_Failure_TopLevelCodeDoesNotReplaceTheNestedCanonicalCopy_Test()
        {
            using var host = await BuildMinimalHost(ep => ep.MapGet("/fail", () =>
                ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse()));

            var raw = await (await host.GetTestClient().GetAsync("/fail")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, ProblemDetailsCodeFixture.ReadCode(problem));
            Assert.IsNotNull(
                problem["extensions"]?["ResultMessages"],
                $"'code' is a re-projection; it must not displace extensions.ResultMessages. Body: {raw}");
            Assert.AreEqual(
                3,
                ProblemDetailsCodeFixture.ReadResultMessageKeys(problem).Count);
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
                            .AddApplicationPart(typeof(ProblemDetailsCodeFunctionalController).Assembly);
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
