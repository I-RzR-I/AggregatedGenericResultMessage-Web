#region U S I N G

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RzR.ResultMessage.Web.Tests.Controllers;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultBaseApiControllerTests
    {
        [TestMethod]
        public async Task JsonResult_HostRegistersXmlFormatters_ClientAcceptsXml_StillReturnsJson_Test()
        {
            using var host = await BuildHost(registerXmlFormatters: true);
            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Accept.ParseAdd("application/xml");

            var response = await client.GetAsync("/api/result-base/json-result");
            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType,
                $"payload was content-negotiated away from JSON. Body: {body}");
            Assert.AreEqual("42", body);
        }

        [TestMethod]
        public async Task JsonWholeResult_HostRegistersXmlFormatters_ClientAcceptsXml_StillReturnsJson_Test()
        {
            using var host = await BuildHost(registerXmlFormatters: true);
            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Accept.ParseAdd("application/xml");

            var response = await client.GetAsync("/api/result-base/json-whole");
            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType,
                $"payload was content-negotiated away from JSON. Body: {body}");
            StringAssert.Contains(body, "\"response\":42");
        }

        [TestMethod]
        public async Task JsonResult_Failure_Returns400WithMessages_Test()
        {
            using var host = await BuildHost();
            var response = await host.GetTestClient().GetAsync("/api/result-base/json-result-fail");
            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            StringAssert.Contains(body, "E001");
        }

        [TestMethod]
        public async Task JsonResultWithNullCheck_NullPayload_Returns204_Test()
        {
            using var host = await BuildHost();
            var response = await host.GetTestClient().GetAsync("/api/result-base/json-result-null-check");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task JsonResult_NonGenericSuccess_Returns204_Test()
        {
            using var host = await BuildHost();
            var response = await host.GetTestClient().GetAsync("/api/result-base/non-generic");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task JsonResult_NullPayloadWithoutNullCheck_Returns200WithNullBody_Test()
        {
            using var host = await BuildHost();
            var response = await host.GetTestClient().GetAsync("/api/result-base/json-result-null");
            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("null", body);
        }

        private static Task<IHost> BuildHost(bool registerXmlFormatters = false)
        {
            return new HostBuilder()
                .ConfigureWebHost(web =>
                {
                    web.UseTestServer();
                    web.ConfigureServices(services =>
                    {
                        services.AddRouting();

                        var mvc = services.AddControllers()
                            .AddApplicationPart(typeof(ResultBaseApiTestController).Assembly);

                        if (registerXmlFormatters)
                            mvc.AddXmlSerializerFormatters();
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
