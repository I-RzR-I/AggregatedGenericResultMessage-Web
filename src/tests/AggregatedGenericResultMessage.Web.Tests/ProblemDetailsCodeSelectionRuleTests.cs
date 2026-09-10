#region U S I N G

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Abstractions.Models;
using RzR.ResultMessage.Enums;
using RzR.ResultMessage.Models;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Factories;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeSelectionRuleTests
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
        public void WithErrorException_ProducesAnExceptionTypedMessageCarryingTheCodeAsKey_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.ExceptionFirstThenKeyedError();

            Assert.AreEqual(
                MessageType.Exception,
                sut.Messages.First().MessageType,
                "Test data guard: WithError(Exception, code) must produce an Exception-typed message, "
                + "otherwise none of the skip/fallback assertions below exercise the intended branch.");
            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ExceptionKey,
                sut.Messages.First().Key,
                "Test data guard: the Exception-typed message really does carry a key that the selection "
                + "rule is required to pass over.");
            Assert.AreNotEqual(
                MessageType.Exception,
                sut.Messages.Last().MessageType,
                "Test data guard: the second message must not be Exception-typed.");
            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ErrorKey,
                sut.Messages.Last().Key,
                "Test data guard: the second message carries the key the rule is expected to select.");
        }

        [TestMethod]
        public async Task Code_LeadingExceptionTypedMessage_IsSkipped_AndTheNextMessageKeyIsEmitted_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.ExceptionFirstThenKeyedError();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ErrorKey,
                problem.Value<string>("code"),
                "Exception-typed messages are passed over by the selection rule, so the code must come "
                + "from the first non-Exception message. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_LeadingExceptionTypedMessage_ItsOwnKeyIsNeverEmittedAsTheCode_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.ExceptionFirstThenKeyedError();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.AreNotEqual(
                ProblemDetailsCodeExceptionTypeFixture.ExceptionKey,
                problem.Value<string>("code"),
                "The skipped Exception-typed message must never supply the code. Body: " + Escape(body));
            Assert.IsFalse(
                body.Contains("\"code\":\"" + ProblemDetailsCodeExceptionTypeFixture.ExceptionKey + "\""),
                "No serialized form of the skipped key may appear as the code member. Body: " + Escape(body));
            CollectionAssert.Contains(
                NestedMessageKeys(problem),
                ProblemDetailsCodeExceptionTypeFixture.ExceptionKey,
                "Only the scalar code member skips the exception; its key must stay visible under "
                + "extensions.ResultMessages. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_LeadingExceptionTypedMessage_CodeAndTitleStillDescribeTheSameMessage_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.ExceptionFirstThenKeyedError();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ErrorInfo,
                problem.Value<string>("title"),
                "Upstream title selection also skips Exception-typed messages. Body: " + Escape(body));
            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ErrorKey,
                problem.Value<string>("code"),
                "code and title must be projections of the same message. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_LeadingExceptionTypedMessage_FollowedByAnUnkeyedError_OmitsCode_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.ExceptionFirstThenUnkeyedError();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "The selected message is the unkeyed non-Exception one, so no code may be emitted - and "
                + "the skipped exception key must not be reached back for. Body: " + Escape(body));
            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ErrorInfo,
                problem.Value<string>("title"),
                "title must still describe the selected message. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_AllMessagesExceptionTyped_FallsBackToTheFirstMessageKey_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.AllExceptionTypedAndKeyed();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ExceptionKey,
                problem.Value<string>("code"),
                "When every message is Exception-typed the rule falls back to the positionally first "
                + "message. Body: " + Escape(body));
            Assert.AreNotEqual(
                ProblemDetailsCodeExceptionTypeFixture.SecondExceptionKey,
                problem.Value<string>("code"),
                "The fallback is positional, so a later exception key must never win. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_AllMessagesExceptionTyped_CodeDescribesTheSameMessageThatSuppliesTitle_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.AllExceptionTypedAndKeyed();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.AreEqual(
                sut.Messages.First().Message?.Info,
                problem.Value<string>("title"),
                "Test data guard: upstream also falls back to the first message for title. Body: "
                + Escape(body));
            Assert.AreEqual(
                sut.Messages.First().Key,
                problem.Value<string>("code"),
                "The fallback branch must keep code and title on the same message. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_AllMessagesExceptionTypedAndUnkeyed_OmitsCode_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.AllExceptionTypedAndUnkeyed();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "The fallback message carries no key, so the member must be omitted rather than emitted "
                + "blank. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_ExceptionTypedMessageFollowedByANullElement_OmitsCode_AndDoesNotThrow_Test()
        {
            var sut = ProblemDetailsCodeExceptionTypeFixture.ExceptionThenNullElement();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsNull(
                sut.GetFirstMessageWithDetails(),
                "Test data guard: upstream selection does not skip null elements, so it supplies no "
                + "title or detail here.");
            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "A null element is the selected message, not a gap to be skipped back over. Body: "
                + Escape(body));
            Assert.IsNull(
                problem.Value<string>("title"),
                "title is absent for the same reason code is. Body: " + Escape(body));
        }

        [TestMethod]
        public void Code_MessageKeyGetterThrows_DoesNotThrow_OmitsCode_AndLeavesTitleAndDetailIntact_Test()
        {
            var sut = new Result { IsSuccess = false };
            sut.Messages = new List<IMessageModel> { new ThrowingKeyMessage() };

            Assert.ThrowsException<InvalidOperationException>(
                () => sut.Messages.First().Key,
                "Test data guard: the stub's Key getter must throw, otherwise the guard is never exercised.");
            Assert.IsNotNull(
                sut.GetFirstMessageWithDetails(),
                "Test data guard: upstream selection reads MessageType and Message only, so title and "
                + "detail must still resolve from the very same message.");

            var objectResult = new DefaultProblemDetailsResultFactory().Create(new ResultProblemDetailsContext
            {
                Result = sut,
                StatusCode = HttpStatusCode.BadRequest,
                HasResponseBody = false
            });

            var problem = objectResult.Value as ResultMessageProblemDetails;

            Assert.IsNotNull(
                problem,
                "The factory must still produce a problem-details body when code selection fails.");
            Assert.IsNull(
                problem.Code,
                "A throwing Key getter must omit the code member instead of failing the error response.");
            Assert.AreEqual(
                ThrowingKeyMessage.Info,
                problem.Title,
                "title is resolved through the upstream guarded selection and must be unaffected.");
            Assert.AreEqual(
                sut.GetFirstMessageWithDetails().ToString(),
                problem.Detail,
                "detail is resolved through the upstream guarded selection and must be unaffected.");
        }

        [TestMethod]
        public async Task Code_SubclassOverridingCreateWholesale_BypassesSanitization_DocumentedLimitation_Test()
        {
            ProblemDetailsResultFactory.Current = new CreateOverridingFactory();

            var sut = ProblemDetailsCodeFixture.MultiMessageFailure();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.AreEqual(
                CreateOverridingFactory.InjectedCode,
                problem.Value<string>("code"),
                "Both sanitize call sites live inside the base Create, so a subclass that replaces Create "
                + "wholesale owns validation entirely. Pinning the boundary so it is an explicit, "
                + "documented limitation rather than an assumed guarantee. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_SubclassOverridingResolveCodeWithJunk_IsStillSanitizedOnAKeyedResult_Test()
        {
            ProblemDetailsResultFactory.Current = new HostileCodeFactory { CodeToReturn = "has space" };

            var sut = ProblemDetailsCodeFixture.MultiMessageFailure();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "A hostile ResolveCode override must not be able to reintroduce a rejected value, and it "
                + "must not fall back to the result's own valid key either. Body: " + Escape(body));
        }

        [TestMethod]
        public async Task Code_SubclassStampingJunkFromApplyExtensions_OverridesAValidResolvedCode_AndIsRejected_Test()
        {
            ProblemDetailsResultFactory.Current = new CodeStampingFactory();

            var sut = ProblemDetailsCodeFixture.MultiMessageFailure();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "The post-ApplyExtensions re-sanitize must reject the stamped value even though a valid "
                + "code had already been resolved from the result. Body: " + Escape(body));
            Assert.IsFalse(
                body.Contains("alert"),
                "No fragment of the stamped payload may reach the wire. Body: " + Escape(body));
        }

        private static string Escape(string raw) => ProblemDetailsCodePairingFixture.Escape(raw);

        private static List<string> NestedMessageKeys(JObject problem)
        {
            var messages = problem["extensions"]?["ResultMessages"] as JArray;

            Assert.IsNotNull(
                messages,
                "extensions.ResultMessages is the canonical message record and must be present.");

            return messages
                .OfType<JObject>()
                .Select(message => message
                    .Properties()
                    .FirstOrDefault(property => string.Equals(property.Name, "key", StringComparison.OrdinalIgnoreCase))
                    ?.Value?.Value<string>())
                .ToList();
        }

        private static async Task<string> RenderAsync(ObjectResult objectResult)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddControllers();

            var bodyStream = new MemoryStream();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider(),
                Response = { Body = bodyStream }
            };

            await objectResult.ExecuteResultAsync(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));

            bodyStream.Position = 0;
            using var reader = new StreamReader(bodyStream, leaveOpen: true);

            return await reader.ReadToEndAsync();
        }

        private sealed class ThrowingKeyMessage : IMessageModel
        {
            internal const string Info = "THROWING-KEY-INFO";

            public string Key
            {
                get => throw new InvalidOperationException("Key getter deliberately throws.");
                set => throw new NotSupportedException();
            }

            public MessageDataModel Message { get; set; } = new MessageDataModel(Info);

            public MessageType MessageType { get; set; } = MessageType.Error;

            public string LogTraceId { get; set; }

            public List<RelatedObjectModel> RelatedObjects { get; set; }
        }
    }
}
