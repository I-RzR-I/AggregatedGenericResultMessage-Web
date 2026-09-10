#region U S I N G

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Abstractions.Models;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeSanitizerTests
    {
        private static readonly string MaxLengthKey = "A.B_C-" + new string('D', 52) + "012345";

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
        public async Task Code_KeyOfExactly64Characters_IsEmittedUnchanged_Test()
        {
            Assert.AreEqual(64, MaxLengthKey.Length, "Test data guard: the boundary key must be exactly 64 characters.");

            var body = await RenderProblemForKeyAsync(MaxLengthKey);
            var problem = JObject.Parse(body);

            Assert.AreEqual(MaxLengthKey, problem.Value<string>("code"),
                "64 characters is the inclusive upper bound and must be accepted verbatim.");
            StringAssert.Contains(body, "\"code\":\"" + MaxLengthKey + "\"",
                "The value must reach the wire unchanged, not merely be present on the model.");
        }

        [TestMethod]
        public async Task Code_KeyOfExactly65Characters_IsOmittedEntirely_Test()
        {
            var overlongKey = MaxLengthKey + "E";
            Assert.AreEqual(65, overlongKey.Length, "Test data guard: the over-boundary key must be exactly 65 characters.");

            var body = await RenderProblemForKeyAsync(overlongKey);
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"], "65 characters is one over the bound and the member must be omitted.");
            Assert.IsFalse(body.Contains("\"code\""), "No 'code' member may appear in the payload at all.");
            CollectionAssert.DoesNotContain(TopLevelPropertyNames(problem), "code");
        }

        [TestMethod]
        public async Task Code_KeyOfExactly65Characters_IsNeverTruncatedToTheBoundary_Test()
        {
            var overlongKey = MaxLengthKey + "E";

            var body = await RenderProblemForKeyAsync(overlongKey);
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.AreEqual(overlongKey, NestedMessageKeys(problem).Single(),
                "The canonical nested key must still carry the full 65-character original.");
            Assert.IsFalse(body.Contains("\"code\":\"" + MaxLengthKey + "\""),
                "A truncated-to-64 projection must never be emitted; the sanitizer validates, it does not transform.");
        }

        [DataTestMethod]
        [DataRow(0x0020, DisplayName = "U+0020 space")]
        [DataRow(0x003A, DisplayName = "U+003A colon")]
        [DataRow(0x002F, DisplayName = "U+002F solidus")]
        [DataRow(0x0023, DisplayName = "U+0023 number sign")]
        [DataRow(0x0022, DisplayName = "U+0022 quotation mark")]
        [DataRow(0x003C, DisplayName = "U+003C less-than sign")]
        [DataRow(0x003E, DisplayName = "U+003E greater-than sign")]
        [DataRow(0x005C, DisplayName = "U+005C reverse solidus")]
        [DataRow(0x002B, DisplayName = "U+002B plus sign")]
        [DataRow(0x0025, DisplayName = "U+0025 percent sign")]
        [DataRow(0x007B, DisplayName = "U+007B left curly bracket")]
        [DataRow(0x002C, DisplayName = "U+002C comma")]
        [DataRow(0x0026, DisplayName = "U+0026 ampersand")]
        [DataRow(0x0000, DisplayName = "U+0000 null character")]
        public async Task Code_KeyWithDisallowedAsciiCharacter_IsOmittedAndNeverRewritten_Test(int codePoint)
        {
            var key = "ORDER" + (char)codePoint + "NOT_FOUND";

            var body = await RenderProblemForKeyAsync(key);
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"],
                $"A key containing U+{codePoint:X4} must be rejected outright.");
            Assert.IsFalse(body.Contains("\"code\""),
                "No partial, stripped or escaped variant of the key may be emitted as 'code'.");
            Assert.AreEqual(key, NestedMessageKeys(problem).Single(),
                "The canonical nested key must still carry the original, unmodified value.");
        }

        [DataTestMethod]
        [DataRow(0x00E9, DisplayName = "U+00E9 latin small letter e with acute")]
        [DataRow(0xFF11, DisplayName = "U+FF11 fullwidth digit one")]
        [DataRow(0x041A, DisplayName = "U+041A cyrillic capital letter ka")]
        [DataRow(0x00B5, DisplayName = "U+00B5 micro sign")]
        [DataRow(0x0660, DisplayName = "U+0660 arabic-indic digit zero")]
        [DataRow(0x00AD, DisplayName = "U+00AD soft hyphen")]
        [DataRow(0x2011, DisplayName = "U+2011 non-breaking hyphen")]
        [DataRow(0x2024, DisplayName = "U+2024 one dot leader")]
        [DataRow(0x00A0, DisplayName = "U+00A0 no-break space")]
        [DataRow(0x2028, DisplayName = "U+2028 line separator")]
        public async Task Code_KeyWithNonAsciiCharacter_IsRejected_WithNoUnicodeLeniency_Test(int codePoint)
        {
            var key = "CODE" + (char)codePoint;

            var body = await RenderProblemForKeyAsync(key);
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"],
                $"Unicode leniency must not be present: a key containing U+{codePoint:X4} must be rejected.");
            Assert.IsFalse(body.Contains("\"code\""),
                "The allowed ASCII prefix must not be stripped out and emitted on its own.");
            Assert.IsFalse(body.Contains("\"code\":\"CODE\""),
                "A prefix-truncated projection must never be emitted; the sanitizer validates, it does not transform.");
        }

        [TestMethod]
        public async Task Code_RejectedKey_OmitsTopLevelCode_ButNestedResultMessageKeyKeepsTheOriginal_Test()
        {
            const string hostileKey = "order id <script>alert(1)</script>";

            var body = await RenderProblemForKeyAsync(hostileKey);
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"],
                "Half one: the rejected value must not be projected to the top level.");
            Assert.IsFalse(body.Contains("\"code\""),
                "Half one: not even an edited version of the rejected value may appear as 'code'.");
            Assert.AreEqual(hostileKey, NestedMessageKeys(problem).Single(),
                "Half two: the canonical nested key must be the original, unmodified value.");
        }

        [TestMethod]
        public async Task Code_AcceptedKey_AgreesExactlyWithTheNestedResultMessageKey_Test()
        {
            const string validKey = "ORDER_NOT_FOUND";

            var problem = JObject.Parse(await RenderProblemForKeyAsync(validKey));

            Assert.AreEqual(validKey, problem.Value<string>("code"));
            Assert.AreEqual(problem.Value<string>("code"), NestedMessageKeys(problem).Single());
        }

        [TestMethod]
        public async Task Code_RejectedFirstKey_DoesNotFallBackToALaterValidKey_Test()
        {
            var sut = new Result { IsSuccess = false }
                .WithError("bad input", "HAS SPACE")
                .WithError("order missing", "ORDER_NOT_FOUND");

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"],
                "The selected message's key is rejected, so no code is emitted; later messages are never consulted.");

            var nestedKeys = NestedMessageKeys(problem);
            CollectionAssert.Contains(nestedKeys, "HAS SPACE");
            CollectionAssert.Contains(nestedKeys, "ORDER_NOT_FOUND");
        }

        [DataTestMethod]
        [DataRow(0x0020, 3, DisplayName = "three spaces")]
        [DataRow(0x0009, 1, DisplayName = "single tab")]
        [DataRow(0x000D, 1, DisplayName = "carriage return")]
        [DataRow(0x000A, 1, DisplayName = "line feed")]
        [DataRow(0x000B, 1, DisplayName = "vertical tab")]
        [DataRow(0x000C, 1, DisplayName = "form feed")]
        [DataRow(0x0020, 0, DisplayName = "empty string")]
        public async Task Code_KeyThatIsBlank_IsOmitted_Test(int codePoint, int repeat)
        {
            var key = new string((char)codePoint, repeat);

            var body = await RenderProblemForKeyAsync(key);
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"], "A blank key must never surface as a code.");
            Assert.IsFalse(body.Contains("\"code\""),
                "An empty or whitespace 'code' would make the two serializer paths disagree; the member must be absent.");
        }

        [TestMethod]
        public async Task Code_KeyOfMixedWhitespace_IsOmitted_Test()
        {
            var key = new string(new[] { ' ', '\t', ' ', '\r', '\n', ' ' });

            var body = await RenderProblemForKeyAsync(key);
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("\"code\""));
        }

        [TestMethod]
        public async Task Code_ContextResultIsNull_DoesNotThrow_AndOmitsCode_Test()
        {
            var factory = new DefaultProblemDetailsResultFactory();

            var objectResult = factory.Create(new ResultProblemDetailsContext
            {
                Result = null,
                StatusCode = HttpStatusCode.InternalServerError,
                HasResponseBody = false
            });

            var body = await RenderAsync(objectResult);
            var problem = JObject.Parse(body);

            Assert.IsNull(((ResultMessageProblemDetails)objectResult.Value).Code);
            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("\"code\""));
        }

        [TestMethod]
        public async Task Code_ResultMessagesCollectionIsNull_DoesNotThrow_AndOmitsCode_Test()
        {
            var sut = new Result { IsSuccess = false }.WithError("boom", "ORDER_NOT_FOUND");
            sut.Messages = null;

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("\"code\""));
        }

        [TestMethod]
        public async Task Code_ResultMessagesCollectionIsEmpty_OmitsCode_Test()
        {
            var sut = new Result { IsSuccess = false }.WithError("boom", "ORDER_NOT_FOUND");
            sut.Messages.Clear();

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("\"code\""));
        }

        [TestMethod]
        public async Task Code_MessageKeyIsNull_DoesNotThrow_AndOmitsCode_Test()
        {
            var sut = new Result { IsSuccess = false }.WithError("boom", null);

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("\"code\""));
            Assert.IsNull(NestedMessageKeys(problem).Single(),
                "Test data guard: the message really does carry a null key.");
        }

        [TestMethod]
        public async Task Code_MessagesCollectionContainsNullElement_DoesNotThrow_AndOmitsCode_Test()
        {
            var sut = new Result { IsSuccess = false };
            sut.Messages = new List<IMessageModel> { null };

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("\"code\""));
        }

        [TestMethod]
        public async Task Code_MessagesCollectionHasNullBeforeAKeyedMessage_OmitsCode_MatchingTitle_Test()
        {
            var keyed = new Result { IsSuccess = false }.WithError("boom", "ORDER_NOT_FOUND");
            var sut = new Result { IsSuccess = false };
            sut.Messages = new List<IMessageModel> { null, keyed.Messages.Single() };

            var body = await RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
            var problem = JObject.Parse(body);

            Assert.IsNull(sut.GetFirstMessageWithDetails(),
                "Test data guard: upstream selects the leading null element, so it supplies no title or "
                + "detail. Code must reach the same verdict instead of skipping ahead to a later key.");
            Assert.IsFalse(body.Contains("\"code\""),
                $"A null element is the selected message, not a gap to be skipped. Body: {body}");
            Assert.IsNull(problem.Value<string>("title"),
                $"Test data guard: title is absent for the same reason code is. Body: {body}");
        }

        [TestMethod]
        public async Task Code_SubclassResolveCodeReturnsValidValue_IsEmitted_ProvingTheHookIsWired_Test()
        {
            var factory = new HostileCodeFactory { CodeToReturn = "SUBCLASS_CODE" };

            var problem = JObject.Parse(await RenderAsync(CreateProblem(factory)));

            Assert.AreEqual("SUBCLASS_CODE", problem.Value<string>("code"),
                "The overridden ResolveCode hook must actually be consulted by the base Create.");
        }

        [TestMethod]
        public async Task Code_SubclassResolveCodeReturnsOverlongValue_IsStillSanitized_Test()
        {
            var factory = new HostileCodeFactory { CodeToReturn = new string('a', 200) };

            var body = await RenderAsync(CreateProblem(factory));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"],
                "Sanitization is applied at the Create call site, so overriding ResolveCode must not bypass it.");
            Assert.IsFalse(body.Contains("\"code\""));
            Assert.IsFalse(body.Contains(new string('a', 65)),
                "No truncated remainder of the 200-character value may reach the wire.");
        }

        [TestMethod]
        public async Task Code_SubclassResolveCodeReturnsScriptPayload_IsStillSanitized_Test()
        {
            var factory = new HostileCodeFactory { CodeToReturn = "<script>alert(1)</script>" };

            var body = await RenderAsync(CreateProblem(factory));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("script"), "No fragment of the injected payload may reach the wire.");
            Assert.IsFalse(body.Contains("alert"));
        }

        [DataTestMethod]
        [DataRow("   ", DisplayName = "spaces only")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow((string)null, DisplayName = "null")]
        public async Task Code_SubclassResolveCodeReturnsBlankValue_IsStillSanitized_Test(string resolved)
        {
            var factory = new HostileCodeFactory { CodeToReturn = resolved };

            var body = await RenderAsync(CreateProblem(factory));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"]);
            Assert.IsFalse(body.Contains("\"code\""));
        }

        [TestMethod]
        public async Task Code_SubclassApplyExtensionsAssignsCodeDirectly_IsStillSanitized_Test()
        {
            var factory = new CodeStampingFactory();

            var body = await RenderAsync(CreateProblem(factory));
            var problem = JObject.Parse(body);

            Assert.IsNull(problem["code"],
                "A value stamped from ApplyExtensions must be sanitized like any other, not passed through.");
            Assert.IsFalse(body.Contains("\"code\""), "The rejected member must be absent, not blank.");
            Assert.IsFalse(body.Contains("script"), "No fragment of the injected payload may reach the wire.");
            Assert.IsFalse(body.Contains("alert"));
        }

        [TestMethod]
        public async Task Code_SubclassApplyExtensionsAssignsWellFormedCode_IsStillEmitted_Test()
        {
            var factory = new WellFormedCodeStampingFactory();

            var problem = JObject.Parse(await RenderAsync(CreateProblem(factory)));

            Assert.AreEqual(WellFormedCodeStampingFactory.InjectedCode, problem.Value<string>("code"),
                "Re-sanitizing after ApplyExtensions must not discard a value the sanitizer accepts.");
        }

        [DataTestMethod]
        [DataRow("A", DisplayName = "single letter")]
        [DataRow("0", DisplayName = "single digit")]
        [DataRow("-", DisplayName = "single hyphen")]
        [DataRow("_", DisplayName = "single underscore")]
        [DataRow(".", DisplayName = "single dot")]
        [DataRow("---", DisplayName = "hyphens only")]
        [DataRow("...", DisplayName = "dots only")]
        [DataRow("___", DisplayName = "underscores only")]
        [DataRow("1234567890", DisplayName = "all digits")]
        [DataRow("order.not-found_v2", DisplayName = "mixed allowed punctuation")]
        [DataRow("aBcDeF", DisplayName = "mixed case is not normalized")]
        [DataRow("-leading-and-trailing-", DisplayName = "leading and trailing hyphen")]
        [DataRow("..--__", DisplayName = "allowed punctuation only")]
        public async Task Code_ValidButUnusualKey_IsAcceptedUnchanged_Test(string key)
        {
            var body = await RenderProblemForKeyAsync(key);
            var problem = JObject.Parse(body);

            Assert.AreEqual(key, problem.Value<string>("code"),
                $"Over-strict validation is as much a defect as under-strict validation: '{key}' is allowed.");
            StringAssert.Contains(body, "\"code\":\"" + key + "\"",
                "The accepted value must reach the wire byte-for-byte.");
        }

        private static ObjectResult CreateProblem(DefaultProblemDetailsResultFactory factory)
            => factory.Create(new ResultProblemDetailsContext
            {
                Result = null,
                StatusCode = HttpStatusCode.BadRequest,
                HasResponseBody = false
            });

        private static Task<string> RenderProblemForKeyAsync(string key)
        {
            var sut = new Result { IsSuccess = false }.WithError("failure text", key);

            return RenderAsync(sut.AsProblemDetails(HttpStatusCode.BadRequest));
        }

        private static List<string> TopLevelPropertyNames(JObject problem)
            => problem.Properties().Select(property => property.Name).ToList();

        private static List<string> NestedMessageKeys(JObject problem)
        {
            var messages = problem["extensions"]?["ResultMessages"] as JArray;

            Assert.IsNotNull(messages,
                "extensions.ResultMessages is the canonical message record and must be present for this assertion.");

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
    }
}
