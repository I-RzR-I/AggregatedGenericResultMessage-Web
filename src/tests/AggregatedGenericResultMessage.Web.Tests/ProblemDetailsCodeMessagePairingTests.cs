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

using HttpIResult = Microsoft.AspNetCore.Http.IResult;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeMessagePairingTests
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

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass)]
        [DataRow(PairingShape.GenericClass)]
        [DataRow(PairingShape.NonGenericInterface)]
        [DataRow(PairingShape.GenericInterface)]
        public async Task AsProblemDetails_SingleKeyedMessage_CodeAndTitleDescribeTheSameMessage_Test(
            PairingShape shape)
        {
            await AssertCodeAndTitleDescribeTheSameMessage(shape, PairingScenario.SingleKeyed);
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass)]
        [DataRow(PairingShape.GenericClass)]
        [DataRow(PairingShape.NonGenericInterface)]
        [DataRow(PairingShape.GenericInterface)]
        public async Task AsProblemDetails_FirstMessageUnkeyedSecondKeyed_OmitsCodeInsteadOfTakingTheLaterKey_Test(
            PairingShape shape)
        {
            var sut = BuildForShape(shape, PairingScenario.LeadingUnkeyedThenKeyed);
            var raw = await ProblemDetailsCodePairingFixture.RenderObjectResultAsync(
                AsProblemDetailsForShape(shape, sut));
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodePairingFixture.HasCodeMember(problem),
                "The message that supplies title and detail carries no key, so code must be omitted "
                + "rather than reached forward for. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
            Assert.AreEqual(
                ProblemDetailsCodePairingFixture.UnkeyedFirstInfo,
                problem.Value<string>("title"),
                "title must still describe the first message when no code is emitted.");
            StringAssert.Contains(
                problem.Value<string>("detail"),
                ProblemDetailsCodePairingFixture.UnkeyedFirstInfo,
                "detail must still describe the first message when no code is emitted.");
            StringAssert.Contains(
                raw,
                ProblemDetailsCodePairingFixture.LateKey,
                "Only the scalar code member is withheld; the later key must stay visible under "
                + "extensions.ResultMessages. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass)]
        [DataRow(PairingShape.GenericClass)]
        [DataRow(PairingShape.NonGenericInterface)]
        [DataRow(PairingShape.GenericInterface)]
        public async Task AsProblemDetails_AllMessagesKeyed_CodeAndTitleDescribeTheSameMessage_Test(
            PairingShape shape)
        {
            await AssertCodeAndTitleDescribeTheSameMessage(shape, PairingScenario.AllKeyed);
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass)]
        [DataRow(PairingShape.GenericClass)]
        [DataRow(PairingShape.NonGenericInterface)]
        [DataRow(PairingShape.GenericInterface)]
        public async Task AsProblemDetails_NoKeysAtAll_OmitsCodeButStillPopulatesTitleAndDetail_Test(
            PairingShape shape)
        {
            var sut = BuildForShape(shape, PairingScenario.NoKeysAtAll);
            var raw = await ProblemDetailsCodePairingFixture.RenderObjectResultAsync(
                AsProblemDetailsForShape(shape, sut));
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodePairingFixture.HasCodeMember(problem),
                "No message carries a key, so the code member must be omitted entirely. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
            Assert.AreEqual(
                ProblemDetailsCodePairingFixture.NoKeyFirstInfo,
                problem.Value<string>("title"),
                "title must still describe the first message even when no code is emitted.");
            StringAssert.Contains(
                problem.Value<string>("detail"),
                ProblemDetailsCodePairingFixture.NoKeyFirstInfo,
                "detail must still describe the first message even when no code is emitted.");
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass)]
        [DataRow(PairingShape.GenericClass)]
        [DataRow(PairingShape.NonGenericInterface)]
        [DataRow(PairingShape.GenericInterface)]
        public async Task AsProblemDetails_AllMessagesKeyed_DetailDescribesTheSameMessageAsTheEmittedCode_Test(
            PairingShape shape)
        {
            var sut = BuildForShape(shape, PairingScenario.AllKeyed);
            var raw = await ProblemDetailsCodePairingFixture.RenderObjectResultAsync(
                AsProblemDetailsForShape(shape, sut));
            var problem = JObject.Parse(raw);

            var code = ProblemDetailsCodePairingFixture.ReadCode(problem);

            Assert.IsNotNull(
                code,
                "The message supplying title and detail is keyed, so a code must be emitted. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));

            var ownerInfo = ProblemDetailsCodePairingFixture.InfoOfMessageOwningKey(sut, code);

            Assert.IsNotNull(
                ownerInfo,
                $"The emitted code '{code}' does not correspond to any message on the result. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
            StringAssert.Contains(
                problem.Value<string>("detail"),
                ownerInfo,
                $"detail must describe the message that owns the emitted code '{code}'. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass, false)]
        [DataRow(PairingShape.GenericClass, true)]
        [DataRow(PairingShape.NonGenericInterface, false)]
        [DataRow(PairingShape.GenericInterface, true)]
        public void AsProblemDetails_EachShape_ReachesTheIntendedFactoryPath_Test(
            PairingShape shape, bool expectedHasResponseBody)
        {
            var recorder = new OverloadRecordingFactory();
            ProblemDetailsResultFactory.Current = recorder;

            var sut = BuildForShape(shape, PairingScenario.SingleKeyed);
            AsProblemDetailsForShape(shape, sut);

            Assert.AreEqual(1, recorder.CreateCallCount, "Exactly one factory invocation is expected.");
            Assert.IsTrue(recorder.RecordedHasResponseBody.HasValue, "The factory must have been invoked.");
            Assert.AreEqual(
                expectedHasResponseBody,
                recorder.RecordedHasResponseBody.Value,
                "HasResponseBody is set only by the generic BuildObjectResult overload, so it proves at "
                + "runtime which of the two factory entry paths the compile-time binding selected.");
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass, "Result")]
        [DataRow(PairingShape.GenericClass, "Result`1")]
        [DataRow(PairingShape.NonGenericInterface, "IResult")]
        [DataRow(PairingShape.GenericInterface, "IResult`1")]
        public void AsProblemDetails_EachShape_HasTheIntendedCompileTimeReceiverType_Test(
            PairingShape shape, string expectedReceiverTypeName)
        {
            Assert.AreEqual(
                expectedReceiverTypeName,
                StaticReceiverTypeNameForShape(shape),
                "Extension-method overload resolution is driven by the compile-time type of the receiver, "
                + "so pinning that type pins which overload the call site binds to.");
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericClass)]
        [DataRow(PairingShape.GenericClass)]
        [DataRow(PairingShape.NonGenericInterface)]
        [DataRow(PairingShape.GenericInterface)]
        public async Task ToProblemResponse_AllMessagesKeyed_CodeAndTitleDescribeTheSameMessage_Test(
            PairingShape shape)
        {
            using var host = await BuildHost();

            var raw = await ReadBodyAsync(
                host, $"/problem-response/{(int)shape}/{(int)PairingScenario.AllKeyed}");
            var problem = JObject.Parse(raw);
            var reference = ProblemDetailsCodePairingFixture.BuildNonGeneric(PairingScenario.AllKeyed);

            var code = ProblemDetailsCodePairingFixture.ReadCode(problem);

            Assert.IsNotNull(
                code,
                "The message supplying title and detail is keyed, so a code must be emitted. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));

            var ownerInfo = ProblemDetailsCodePairingFixture.InfoOfMessageOwningKey(reference, code);

            Assert.IsNotNull(
                ownerInfo,
                $"The emitted code '{code}' does not correspond to any message on the result. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
            Assert.AreEqual(
                ownerInfo,
                problem.Value<string>("title"),
                $"title must describe the message that owns the emitted code '{code}'. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
        }

        [DataTestMethod]
        [DataRow(PairingShape.NonGenericInterface)]
        [DataRow(PairingShape.GenericInterface)]
        public async Task ToHttpResult_AllMessagesKeyed_CodeAndTitleDescribeTheSameMessage_Test(
            PairingShape shape)
        {
            using var host = await BuildHost();

            var raw = await ReadBodyAsync(
                host, $"/http-result/{(int)shape}/{(int)PairingScenario.AllKeyed}");
            var problem = JObject.Parse(raw);
            var reference = ProblemDetailsCodePairingFixture.BuildNonGeneric(PairingScenario.AllKeyed);

            var code = ProblemDetailsCodePairingFixture.ReadCode(problem);

            Assert.IsNotNull(
                code,
                "The message supplying title and detail is keyed, so a code must be emitted. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));

            var ownerInfo = ProblemDetailsCodePairingFixture.InfoOfMessageOwningKey(reference, code);

            Assert.IsNotNull(
                ownerInfo,
                $"The emitted code '{code}' does not correspond to any message on the result. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
            Assert.AreEqual(
                ownerInfo,
                problem.Value<string>("title"),
                $"title must describe the message that owns the emitted code '{code}'. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
        }

        [TestMethod]
        public async Task AllFourShapes_ProduceTheIdenticalCodeTitleDetailTriple_ForTheLeadingUnkeyedResult_Test()
        {
            var observed = new List<string>();

            foreach (var shape in AllShapes())
            {
                var sut = BuildForShape(shape, PairingScenario.LeadingUnkeyedThenKeyed);
                var problem = JObject.Parse(await ProblemDetailsCodePairingFixture.RenderObjectResultAsync(
                    AsProblemDetailsForShape(shape, sut)));

                observed.Add(string.Join(
                    " | ",
                    ProblemDetailsCodePairingFixture.ReadCode(problem) ?? "<absent>",
                    problem.Value<string>("title") ?? "<absent>",
                    problem.Value<string>("detail") ?? "<absent>"));
            }

            var distinct = observed.Distinct().ToList();

            Assert.AreEqual(
                1,
                distinct.Count,
                "All four result shapes must resolve the same code/title/detail triple for the same message set. "
                + "Observed: " + ProblemDetailsCodePairingFixture.Escape(string.Join(" || ", distinct)));
        }

        [TestMethod]
        public void FailureWithCodeFirst_AndWithErrorWithCodeSecond_PopulateTheSameKeySlot_Test()
        {
            var viaFailure = Result<string>.Failure(
                ProblemDetailsCodePairingFixture.SingleKey, ProblemDetailsCodePairingFixture.SingleKeyedInfo);

            var viaWithError = new Result<string> { IsSuccess = false };
            viaWithError.WithError(
                ProblemDetailsCodePairingFixture.SingleKeyedInfo, ProblemDetailsCodePairingFixture.SingleKey);

            Assert.AreEqual(
                ProblemDetailsCodePairingFixture.SingleKey,
                viaFailure.Messages.First().Key,
                "Failure(code, error) takes the code first.");
            Assert.AreEqual(
                viaFailure.Messages.First().Key,
                viaWithError.Messages.First().Key,
                "WithError(error, code) takes the code second but must reach the same Key slot.");
            Assert.AreEqual(
                viaFailure.Messages.First().Message?.Info,
                viaWithError.Messages.First().Message?.Info,
                "Both authoring routes must put the human-readable text in the same slot.");
        }

        [TestMethod]
        public async Task FailureWithCodeFirst_AndWithErrorWithCodeSecond_EmitTheSameSerializedCodeAndTitle_Test()
        {
            var viaFailure = Result<string>.Failure(
                ProblemDetailsCodePairingFixture.SingleKey, ProblemDetailsCodePairingFixture.SingleKeyedInfo);

            var viaWithError = new Result<string> { IsSuccess = false };
            viaWithError.WithError(
                ProblemDetailsCodePairingFixture.SingleKeyedInfo, ProblemDetailsCodePairingFixture.SingleKey);

            var failureBody = JObject.Parse(await ProblemDetailsCodePairingFixture.RenderObjectResultAsync(
                viaFailure.AsProblemDetails(HttpStatusCode.BadRequest)));
            var withErrorBody = JObject.Parse(await ProblemDetailsCodePairingFixture.RenderObjectResultAsync(
                viaWithError.AsProblemDetails(HttpStatusCode.BadRequest)));

            Assert.AreEqual(
                ProblemDetailsCodePairingFixture.SingleKey,
                ProblemDetailsCodePairingFixture.ReadCode(failureBody));
            Assert.AreEqual(
                ProblemDetailsCodePairingFixture.ReadCode(failureBody),
                ProblemDetailsCodePairingFixture.ReadCode(withErrorBody));
            Assert.AreEqual(
                failureBody.Value<string>("title"),
                withErrorBody.Value<string>("title"));
        }

        #region Helpers

        private static async Task AssertCodeAndTitleDescribeTheSameMessage(
            PairingShape shape, PairingScenario scenario)
        {
            var sut = BuildForShape(shape, scenario);
            var raw = await ProblemDetailsCodePairingFixture.RenderObjectResultAsync(
                AsProblemDetailsForShape(shape, sut));
            var problem = JObject.Parse(raw);

            Console.WriteLine($"[{shape}/{scenario}] {raw}");

            var code = ProblemDetailsCodePairingFixture.ReadCode(problem);

            Assert.IsNotNull(
                code,
                "Test data guard: this scenario authors at least one keyed message, so a code is expected. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));

            var ownerInfo = ProblemDetailsCodePairingFixture.InfoOfMessageOwningKey(sut, code);

            Assert.IsNotNull(
                ownerInfo,
                $"The emitted code '{code}' does not correspond to any message on the result. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));

            Assert.AreEqual(
                ownerInfo,
                problem.Value<string>("title"),
                $"code '{code}' and title must describe the same message. Body: "
                + ProblemDetailsCodePairingFixture.Escape(raw));
        }

        private static IEnumerable<PairingShape> AllShapes()
        {
            yield return PairingShape.NonGenericClass;
            yield return PairingShape.GenericClass;
            yield return PairingShape.NonGenericInterface;
            yield return PairingShape.GenericInterface;
        }

        private static IResult BuildForShape(PairingShape shape, PairingScenario scenario)
            => shape == PairingShape.GenericClass || shape == PairingShape.GenericInterface
                ? (IResult)ProblemDetailsCodePairingFixture.BuildGeneric(scenario)
                : (IResult)ProblemDetailsCodePairingFixture.BuildNonGeneric(scenario);

        private static ObjectResult AsProblemDetailsForShape(PairingShape shape, IResult built)
        {
            switch (shape)
            {
                case PairingShape.NonGenericClass:
                {
                    var sut = (Result)built;

                    return sut.AsProblemDetails(HttpStatusCode.BadRequest);
                }

                case PairingShape.GenericClass:
                {
                    var sut = (Result<string>)built;

                    return sut.AsProblemDetails(HttpStatusCode.BadRequest);
                }

                case PairingShape.NonGenericInterface:
                {
                    var sut = built;

                    return sut.AsProblemDetails(HttpStatusCode.BadRequest);
                }

                case PairingShape.GenericInterface:
                {
                    var sut = (IResult<string>)built;

                    return sut.AsProblemDetails(HttpStatusCode.BadRequest);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape));
            }
        }

        private static string StaticReceiverTypeNameForShape(PairingShape shape)
        {
            var built = BuildForShape(shape, PairingScenario.SingleKeyed);

            switch (shape)
            {
                case PairingShape.NonGenericClass:
                {
                    var sut = (Result)built;

                    return StaticReceiverTypeOf(sut).Name;
                }

                case PairingShape.GenericClass:
                {
                    var sut = (Result<string>)built;

                    return StaticReceiverTypeOf(sut).Name;
                }

                case PairingShape.NonGenericInterface:
                {
                    var sut = built;

                    return StaticReceiverTypeOf(sut).Name;
                }

                case PairingShape.GenericInterface:
                {
                    var sut = (IResult<string>)built;

                    return StaticReceiverTypeOf(sut).Name;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape));
            }
        }

        private static Type StaticReceiverTypeOf<TReceiver>(TReceiver receiver) => typeof(TReceiver);

        private static ResultMessageResponse ToProblemResponseForShape(PairingShape shape, PairingScenario scenario)
        {
            var built = BuildForShape(shape, scenario);

            switch (shape)
            {
                case PairingShape.NonGenericClass:
                {
                    var sut = (Result)built;

                    return sut.ToProblemResponse(HttpStatusCode.BadRequest);
                }

                case PairingShape.GenericClass:
                {
                    var sut = (Result<string>)built;

                    return sut.ToProblemResponse(HttpStatusCode.BadRequest);
                }

                case PairingShape.NonGenericInterface:
                {
                    var sut = built;

                    return sut.ToProblemResponse(HttpStatusCode.BadRequest);
                }

                case PairingShape.GenericInterface:
                {
                    var sut = (IResult<string>)built;

                    return sut.ToProblemResponse(HttpStatusCode.BadRequest);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape));
            }
        }

        private static HttpIResult ToHttpResultForShape(PairingShape shape, PairingScenario scenario)
        {
            var built = BuildForShape(shape, scenario);

            if (shape == PairingShape.GenericInterface)
            {
                var generic = (IResult<string>)built;

                return generic.ToHttpResult(HttpStatusCode.BadRequest);
            }

            return built.ToHttpResult(HttpStatusCode.BadRequest);
        }

        private static async Task<string> ReadBodyAsync(IHost host, string path)
            => await (await host.GetTestClient().GetAsync(path)).Content.ReadAsStringAsync();

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
                            endpoints.MapGet("/problem-response/{shape:int}/{scenario:int}", (int shape, int scenario) =>
                                ToProblemResponseForShape((PairingShape)shape, (PairingScenario)scenario));

                            endpoints.MapGet("/http-result/{shape:int}/{scenario:int}", (int shape, int scenario) =>
                                ToHttpResultForShape((PairingShape)shape, (PairingScenario)scenario));
                        });
                    });
                })
                .StartAsync();
        }

        #endregion
    }
}
