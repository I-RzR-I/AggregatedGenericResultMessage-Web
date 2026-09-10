#region U S I N G

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeSerializationTests
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
        public async Task Payload_WithCode_HasExactlySevenTopLevelMembers_InTheDocumentedOrder_Test()
        {
            var problem = JObject.Parse(await RenderAsync(BuildFullyPopulatedProblem("ORDER_NOT_FOUND")));

            CollectionAssert.AreEqual(
                new List<string> { "type", "title", "status", "detail", "instance", "code", "extensions" },
                TopLevelPropertyNames(problem),
                "The converter hand-writes an explicit whitelist, so both the member set and its order are contractual. "
                + "A new member appearing here means the payload contract changed.");
        }

        [TestMethod]
        public async Task Payload_WithoutCode_HasExactlySixTopLevelMembers_AndDropsOnlyCode_Test()
        {
            var problem = JObject.Parse(await RenderAsync(BuildFullyPopulatedProblem("REJECTED KEY")));

            CollectionAssert.AreEqual(
                new List<string> { "type", "title", "status", "detail", "instance", "extensions" },
                TopLevelPropertyNames(problem),
                "Omitting the code must remove exactly one member and disturb nothing else.");
        }

        [TestMethod]
        public async Task Payload_CodeMember_IsWrittenAfterInstance_AndBeforeExtensions_Test()
        {
            var problem = JObject.Parse(await RenderAsync(BuildFullyPopulatedProblem("ORDER_NOT_FOUND")));
            var names = TopLevelPropertyNames(problem);

            var instanceIndex = names.IndexOf("instance");
            var codeIndex = names.IndexOf("code");
            var extensionsIndex = names.IndexOf("extensions");

            Assert.AreNotEqual(-1, instanceIndex, "Test data guard: 'instance' must be present.");
            Assert.AreNotEqual(-1, codeIndex, "Test data guard: 'code' must be present.");
            Assert.AreNotEqual(-1, extensionsIndex, "Test data guard: 'extensions' must be present.");

            Assert.IsTrue(instanceIndex < codeIndex, "'code' must be written after 'instance'.");
            Assert.IsTrue(codeIndex < extensionsIndex, "'code' must be written before 'extensions'.");
        }

        [TestMethod]
        public async Task Payload_OtherFiveRfc7807Members_AreUnchangedByTheCodeFeature_Test()
        {
            var problem = JObject.Parse(await RenderAsync(BuildFullyPopulatedProblem("ORDER_NOT_FOUND")));

            StringAssert.Contains(problem.Value<string>("type"), "rfc");
            Assert.AreEqual("Order not found", problem.Value<string>("title"));
            Assert.AreEqual(StatusCodes.Status404NotFound, problem.Value<int>("status"));
            Assert.AreEqual("no order with that id", problem.Value<string>("detail"));
            Assert.AreEqual("/api/orders/42", problem.Value<string>("instance"));
        }

        [TestMethod]
        public async Task Payload_ExtensionsEnvelope_IsUnchangedByTheCodeFeature_Test()
        {
            var withCode = JObject.Parse(await RenderAsync(BuildFullyPopulatedProblem("ORDER_NOT_FOUND")));
            var withoutCode = JObject.Parse(await RenderAsync(BuildFullyPopulatedProblem("REJECTED KEY")));

            CollectionAssert.AreEqual(
                new List<string> { "ResultMessages" },
                ((JObject)withCode["extensions"]).Properties().Select(property => property.Name).ToList(),
                "The code member must not add, rename or remove anything inside the extensions envelope.");

            CollectionAssert.AreEqual(
                ((JObject)withCode["extensions"]).Properties().Select(property => property.Name).ToList(),
                ((JObject)withoutCode["extensions"]).Properties().Select(property => property.Name).ToList(),
                "The extensions envelope must have the same shape whether or not a code was emitted.");
        }

        [TestMethod]
        public async Task Payload_CodeMemberName_IsImmuneToAHostileMvcPropertyNamingPolicy_Test()
        {
            var problem = JObject.Parse(
                await RenderAsync(BuildFullyPopulatedProblem("ORDER_NOT_FOUND"), new HostilePropertyNamingPolicy()));

            Assert.AreEqual("ORDER_NOT_FOUND", problem.Value<string>("code"),
                "The converter writes the literal \"code\", so no PropertyNamingPolicy can rename it.");
            Assert.IsNull(problem["XX_CODE"], "The naming policy must not have been applied to the member name.");
        }

        [TestMethod]
        public async Task Payload_AllHandWrittenMemberNames_AreImmuneToAHostilePropertyNamingPolicy_Test()
        {
            var problem = JObject.Parse(
                await RenderAsync(BuildFullyPopulatedProblem("ORDER_NOT_FOUND"), new HostilePropertyNamingPolicy()));

            CollectionAssert.AreEqual(
                new List<string> { "type", "title", "status", "detail", "instance", "code", "extensions" },
                TopLevelPropertyNames(problem),
                "Every top-level name is a literal in the converter, not a reflected member name.");
        }

        [TestMethod]
        public void Payload_CodeMemberName_IsImmuneToAHostileNamingPolicy_OnDirectSystemTextJsonSerialization_Test()
        {
            var problem = new ResultMessageProblemDetails
            {
                Type = "about:blank",
                Status = StatusCodes.Status400BadRequest,
                Code = "ORDER_NOT_FOUND"
            };

            var json = JsonSerializer.Serialize(problem,
                new JsonSerializerOptions { PropertyNamingPolicy = new HostilePropertyNamingPolicy() });
            var parsed = JObject.Parse(json);

            Assert.AreEqual("ORDER_NOT_FOUND", parsed.Value<string>("code"));
            Assert.IsNull(parsed["XX_CODE"]);
        }

        [DataTestMethod]
        [DataRow((string)null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty string")]
        [DataRow("   ", DisplayName = "spaces only")]
        [DataRow("\t", DisplayName = "tab")]
        [DataRow("\r\n", DisplayName = "carriage return line feed")]
        public void ShouldSerializeCode_ReturnsFalse_WhenCodeIsBlank_Test(string code)
        {
            var problem = new ResultMessageProblemDetails { Code = code };

            Assert.IsFalse(problem.ShouldSerializeCode(),
                "Newtonsoft honours this convention by name alone; returning true here would emit an empty code member.");
        }

        [DataTestMethod]
        [DataRow("A", DisplayName = "single character")]
        [DataRow("ORDER_NOT_FOUND", DisplayName = "typical code")]
        [DataRow(" PADDED ", DisplayName = "value with surrounding whitespace")]
        public void ShouldSerializeCode_ReturnsTrue_WhenCodeHasContent_Test(string code)
        {
            var problem = new ResultMessageProblemDetails { Code = code };

            Assert.IsTrue(problem.ShouldSerializeCode());
        }

        [TestMethod]
        public void ShouldSerializeCode_ReturnsFalse_OnADefaultConstructedInstance_Test()
        {
            var problem = new ResultMessageProblemDetails();

            Assert.IsNull(problem.Code, "Code has no initializer and must default to null, not string.Empty.");
            Assert.IsFalse(problem.ShouldSerializeCode());
        }

        [TestMethod]
        public void Newtonsoft_OmitsCodeMember_WhenCodeIsBlank_ViaTheShouldSerializeConvention_Test()
        {
            var problem = new ResultMessageProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Code = "   "
            };

            var parsed = JObject.Parse(SerializeWithNewtonsoftCamelCase(problem));

            Assert.IsNull(FindPropertyIgnoringCase(parsed, "code"),
                "On any host that has called AddNewtonsoftJson the custom converter never runs, so "
                + "ShouldSerializeCode is the only thing keeping a blank code out of the payload.");
        }

        [TestMethod]
        public void Newtonsoft_EmitsCodeMember_WhenCodeIsPresent_UnderTheAspNetCoreCamelCaseResolver_Test()
        {
            var problem = new ResultMessageProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Code = "ORDER_NOT_FOUND"
            };

            var parsed = JObject.Parse(SerializeWithNewtonsoftCamelCase(problem));

            Assert.AreEqual("ORDER_NOT_FOUND", parsed.Value<string>("code"));
        }

        [TestMethod]
        public void Newtonsoft_DefaultContractResolver_DoesNotPinTheCodeMemberName_KnownContractGap_Test()
        {
            var problem = new ResultMessageProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Code = "ORDER_NOT_FOUND"
            };

            var parsed = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(problem));

            Assert.AreEqual("ORDER_NOT_FOUND", parsed.Value<string>("Code"),
                "Known gap: on every Newtonsoft path the member name is resolver-dependent rather than pinned "
                + "to \"code\"; ASP.NET Core's default camelCase resolver is what makes it \"code\" in practice.");
            Assert.IsNull(parsed["code"]);
        }

        private sealed class HostilePropertyNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) => "XX_" + name.ToUpperInvariant();
        }

        private static ObjectResult BuildFullyPopulatedProblem(string messageKey)
        {
            var sut = new Result { IsSuccess = false }.WithError("order missing", messageKey);

            return sut.AsProblemDetails(
                HttpStatusCode.NotFound,
                "Order not found",
                "no order with that id",
                "/api/orders/42");
        }

        private static string SerializeWithNewtonsoftCamelCase(ResultMessageProblemDetails problem)
            => Newtonsoft.Json.JsonConvert.SerializeObject(problem, new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

        private static JProperty FindPropertyIgnoringCase(JObject source, string name)
            => source.Properties()
                .FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase));

        private static List<string> TopLevelPropertyNames(JObject problem)
            => problem.Properties().Select(property => property.Name).ToList();

        private static async Task<string> RenderAsync(
            ObjectResult objectResult, JsonNamingPolicy propertyNamingPolicy = null)
        {
            var services = new ServiceCollection();
            services.AddLogging();

            var mvcBuilder = services.AddControllers();
            if (propertyNamingPolicy != null)
            {
                mvcBuilder.AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = propertyNamingPolicy);
            }

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
