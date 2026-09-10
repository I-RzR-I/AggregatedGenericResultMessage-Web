#region U S I N G

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Filters;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Controllers;
using RzR.ResultMessage.Web.Tests.Fixtures;
using RzR.ResultMessage.Web.WebDependencyInjection;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeUnhandledOptionTests
    {
        private const string CodeWithSpace = "Internal Server Error";

        private static readonly string MaxLengthCode = "A.B_C-" + new string('D', 52) + "012345";

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
        [DataRow("Internal Server Error", DisplayName = "value containing spaces")]
        [DataRow("INTERNAL:ERROR", DisplayName = "value containing a colon")]
        [DataRow("internal/error", DisplayName = "value containing a solidus")]
        [DataRow("   ", DisplayName = "whitespace only")]
        [DataRow("", DisplayName = "empty string")]
        public async Task Middleware_UnhandledException_WithInvalidDefaultUnhandledErrorCode_OmitsTheCodeMember_Test(
            string configured)
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom"),
                options => options.DefaultUnhandledErrorCode = configured);

            var raw = await ReadBodyAsync(host);
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "DefaultUnhandledErrorCode goes through the same sanitizer as a message key, so an "
                + "invalid configured value must be dropped rather than emitted. Body: " + Escape(raw));
        }

        [TestMethod]
        public async Task Middleware_UnhandledException_WithOverlongDefaultUnhandledErrorCode_OmitsTheCodeMember_Test()
        {
            var overlong = MaxLengthCode + "E";

            Assert.AreEqual(65, overlong.Length, "Test data guard: the value must be one over the bound.");

            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom"),
                options => options.DefaultUnhandledErrorCode = overlong);

            var raw = await ReadBodyAsync(host);
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "A 65-character configured code is one over the inclusive bound. Body: " + Escape(raw));
            Assert.IsFalse(
                raw.Contains("\"code\":\"" + MaxLengthCode + "\""),
                "A truncated-to-64 projection must never be emitted. Body: " + Escape(raw));
        }

        [TestMethod]
        public async Task Middleware_UnhandledException_WithMaximumLengthDefaultUnhandledErrorCode_EmitsIt_Test()
        {
            Assert.AreEqual(64, MaxLengthCode.Length, "Test data guard: the value must be exactly at the bound.");

            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom"),
                options => options.DefaultUnhandledErrorCode = MaxLengthCode);

            var raw = await ReadBodyAsync(host);
            var problem = JObject.Parse(raw);

            Assert.AreEqual(
                MaxLengthCode,
                ProblemDetailsCodeFixture.ReadCode(problem),
                "64 characters is the inclusive upper bound on the unhandled path too. Body: " + Escape(raw));
        }

        [TestMethod]
        public async Task Middleware_UnhandledException_WithInvalidDefaultUnhandledErrorCode_KeepsTheRawValueUnderExtensions_Test()
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom"),
                options => options.DefaultUnhandledErrorCode = CodeWithSpace);

            var raw = await ReadBodyAsync(host);
            var problem = JObject.Parse(raw);

            CollectionAssert.Contains(
                ProblemDetailsCodeFixture.ReadResultMessageKeys(problem).ToList(),
                CodeWithSpace,
                "Validate-never-transform: the rejected value is withheld only from the scalar code "
                + "member, and stays intact on the synthesized message. Body: " + Escape(raw));
        }

        [TestMethod]
        public async Task Middleware_UnhandledException_WithInvalidDefaultUnhandledErrorCode_StillReturns500AndAWellFormedProblemBody_Test()
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom"),
                options => options.DefaultUnhandledErrorCode = CodeWithSpace);

            var response = await host.GetTestClient().GetAsync("/boom");
            var raw = await response.Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(
                StatusCodes.Status500InternalServerError,
                problem.Value<int>("status"),
                "A rejected code must not disturb the rest of the payload. Body: " + Escape(raw));
            Assert.AreEqual(
                "Unhandled exception",
                problem.Value<string>("title"),
                "A rejected code must not disturb the title. Body: " + Escape(raw));
        }

        [TestMethod]
        public async Task Filter_ThrownResultLedByAnExceptionTypedMessage_EmitsTheNextMessageKey_Test()
        {
            using var host = await BuildFilterHost();

            var raw = await (await host.GetTestClient().GetAsync("/api/code-exception/throw-exception-typed"))
                .Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(
                ProblemDetailsCodeExceptionTypeFixture.ErrorKey,
                ProblemDetailsCodeFixture.ReadCode(problem),
                "The filter must apply the same Exception-skipping selection rule. Body: " + Escape(raw));
        }

        [TestMethod]
        public async Task FilterAndMiddleware_AgreeOnTheCode_ForAResultLedByAnExceptionTypedMessage_Test()
        {
            using var middlewareHost = await BuildMiddlewareHost(
                _ => throw new WebResultException(
                    ProblemDetailsCodeExceptionTypeFixture.ExceptionFirstThenKeyedError()));
            using var filterHost = await BuildFilterHost();

            var fromMiddleware = ProblemDetailsCodeFixture.ReadCode(JObject.Parse(await ReadBodyAsync(middlewareHost)));
            var fromFilter = ProblemDetailsCodeFixture.ReadCode(JObject.Parse(
                await (await filterHost.GetTestClient().GetAsync("/api/code-exception/throw-exception-typed"))
                    .Content.ReadAsStringAsync()));

            Assert.AreEqual(ProblemDetailsCodeExceptionTypeFixture.ErrorKey, fromMiddleware);
            Assert.AreEqual(
                fromMiddleware,
                fromFilter,
                "Both exception surfaces must render the same code for the same thrown result.");
        }

        [TestMethod]
        public async Task FilterAndMiddleware_AgreeOnTheCode_ForAResultWhereEveryMessageIsExceptionTyped_Test()
        {
            using var middlewareHost = await BuildMiddlewareHost(
                _ => throw new WebResultException(
                    ProblemDetailsCodeExceptionTypeFixture.AllExceptionTypedAndKeyed()));
            using var filterHost = await BuildFilterHost();

            var fromMiddleware = ProblemDetailsCodeFixture.ReadCode(JObject.Parse(await ReadBodyAsync(middlewareHost)));
            var fromFilter = ProblemDetailsCodeFixture.ReadCode(JObject.Parse(
                await (await filterHost.GetTestClient().GetAsync("/api/code-exception/throw-all-exception-typed"))
                    .Content.ReadAsStringAsync()));

            Assert.AreEqual(ProblemDetailsCodeExceptionTypeFixture.ExceptionKey, fromMiddleware);
            Assert.AreEqual(fromMiddleware, fromFilter);
        }

        [TestMethod]
        public void Filter_NonWebResultException_IsLeftUnhandled_SoItHasNoUnhandledErrorCodePath_Test()
        {
            var exceptionContext = new ExceptionContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>())
            {
                Exception = new InvalidOperationException("boom")
            };

            new WebResultExceptionFilter().OnException(exceptionContext);

            Assert.IsFalse(
                exceptionContext.ExceptionHandled,
                "The filter only converts WebResultException, so DefaultUnhandledErrorCode has no filter "
                + "counterpart - an asymmetry consumers must be aware of when choosing a surface.");
            Assert.IsNull(
                exceptionContext.Result,
                "No ProblemDetails result - and therefore no code member - is produced by the filter for "
                + "a plain exception.");
        }

        #region Helpers

        private static string Escape(string raw) => ProblemDetailsCodePairingFixture.Escape(raw);

        private static async Task<string> ReadBodyAsync(IHost host)
            => await (await host.GetTestClient().GetAsync("/boom")).Content.ReadAsStringAsync();

        private static Task<IHost> BuildMiddlewareHost(
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

        private static Task<IHost> BuildFilterHost()
        {
            return new HostBuilder()
                .ConfigureWebHost(web =>
                {
                    web.UseTestServer();
                    web.ConfigureServices(services =>
                    {
                        services.AddRouting();

                        services.AddControllers()
                            .AddApplicationPart(typeof(ProblemDetailsCodeExceptionController).Assembly);

                        services.AddWebResultExceptionFilter();
                    });
                    web.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapControllers());
                    });
                })
                .StartAsync();
        }

        #endregion
    }
}
