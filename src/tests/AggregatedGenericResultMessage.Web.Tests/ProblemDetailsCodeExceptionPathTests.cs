#region U S I N G

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Controllers;
using RzR.ResultMessage.Web.Tests.Fixtures;
using RzR.ResultMessage.Web.WebDependencyInjection;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeExceptionPathTests
    {
        private const string ConfiguredUnhandledCode = "c-UNHANDLED";

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
        public async Task Middleware_WebResultException_EmitsTheCallersOwnCodeInTheJsonBody_Test()
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new WebResultException(ProblemDetailsCodeFixture.MultiMessageFailure()));

            var raw = await (await host.GetTestClient().GetAsync("/boom")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(
                ProblemDetailsCodeFixture.FirstKey,
                ProblemDetailsCodeFixture.ReadCode(problem),
                $"Middleware did not surface the caller's code. Body: {raw}");
        }

        [TestMethod]
        public async Task Filter_WebResultException_EmitsTheCallersOwnCodeInTheJsonBody_Test()
        {
            using var host = await BuildFilterHost();

            var raw = await (await host.GetTestClient().GetAsync("/api/code-exception/throw-keyed"))
                .Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(
                ProblemDetailsCodeFixture.FirstKey,
                ProblemDetailsCodeFixture.ReadCode(problem),
                $"Filter did not surface the caller's code. Body: {raw}");
        }

        [TestMethod]
        public async Task FilterAndMiddleware_AgreeOnTheCodeForTheSameThrownResult_Test()
        {
            using var middlewareHost = await BuildMiddlewareHost(
                _ => throw new WebResultException(ProblemDetailsCodeFixture.MultiMessageFailure()));
            using var filterHost = await BuildFilterHost();

            var fromMiddleware = ProblemDetailsCodeFixture.ReadCode(JObject.Parse(
                await (await middlewareHost.GetTestClient().GetAsync("/boom")).Content.ReadAsStringAsync()));
            var fromFilter = ProblemDetailsCodeFixture.ReadCode(JObject.Parse(
                await (await filterHost.GetTestClient().GetAsync("/api/code-exception/throw-keyed"))
                    .Content.ReadAsStringAsync()));

            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, fromMiddleware);
            Assert.AreEqual(
                fromMiddleware,
                fromFilter,
                "The MVC filter and the pipeline middleware must render the same code for the same result.");
        }

        [TestMethod]
        public async Task Middleware_WebResultExceptionWithNoKeys_OmitsTheCodeMember_Test()
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new WebResultException(ProblemDetailsCodeFixture.UnkeyedFailure()));

            var raw = await (await host.GetTestClient().GetAsync("/boom")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                $"'code' must be absent when the thrown result carries no key. Body: {raw}");
        }

        [TestMethod]
        public async Task Filter_WebResultExceptionWithNoKeys_OmitsTheCodeMember_Test()
        {
            using var host = await BuildFilterHost();

            var raw = await (await host.GetTestClient().GetAsync("/api/code-exception/throw-unkeyed"))
                .Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                $"'code' must be absent when the thrown result carries no key. Body: {raw}");
        }
        
        [TestMethod]
        public async Task Middleware_UnhandledException_ByDefault_OmitsTheCodeMemberEntirely_Test()
        {
            using var host = await BuildMiddlewareHost(_ => throw new InvalidOperationException("boom"));

            var response = await host.GetTestClient().GetAsync("/boom");
            var raw = await response.Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                "DefaultUnhandledErrorCode defaults to null, so the library must not mint a contract value "
                + $"that consumers would start branching on. Body: {raw}");
        }

        [TestMethod]
        public async Task Middleware_UnhandledException_WithConfiguredDefaultUnhandledErrorCode_EmitsThatCode_Test()
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom"),
                options => options.DefaultUnhandledErrorCode = ConfiguredUnhandledCode);

            var raw = await (await host.GetTestClient().GetAsync("/boom")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(
                ConfiguredUnhandledCode,
                ProblemDetailsCodeFixture.ReadCode(problem),
                $"The consumer-configured unhandled code must reach the wire. Body: {raw}");
        }

        [TestMethod]
        public async Task Middleware_UnhandledException_WithExceptionMessageExposed_StillDoesNotLeakItIntoCode_Test()
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom-secret-internal-detail"),
                options => options.IncludeExceptionMessageInDetail = true);

            var raw = await (await host.GetTestClient().GetAsync("/boom")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual("boom-secret-internal-detail", problem.Value<string>("detail"));
            Assert.IsFalse(
                ProblemDetailsCodeFixture.HasCodeMember(problem),
                $"Exception text must never be routed into the 'code' member. Body: {raw}");
        }

        [TestMethod]
        public async Task Middleware_UnhandledException_WithConfiguredCode_StillEmitsCodeWhenExceptionMessageExposed_Test()
        {
            using var host = await BuildMiddlewareHost(
                _ => throw new InvalidOperationException("boom-secret-internal-detail"),
                options =>
                {
                    options.IncludeExceptionMessageInDetail = true;
                    options.DefaultUnhandledErrorCode = ConfiguredUnhandledCode;
                });

            var raw = await (await host.GetTestClient().GetAsync("/boom")).Content.ReadAsStringAsync();
            var problem = JObject.Parse(raw);

            Assert.AreEqual(ConfiguredUnhandledCode, ProblemDetailsCodeFixture.ReadCode(problem));
        }

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
    }
}
