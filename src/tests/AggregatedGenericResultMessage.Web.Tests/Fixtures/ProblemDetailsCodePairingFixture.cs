#region U S I N G

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

#endregion

namespace RzR.ResultMessage.Web.Tests.Fixtures
{
    public enum PairingShape
    {
        NonGenericClass,
        GenericClass,
        NonGenericInterface,
        GenericInterface
    }

    public enum PairingScenario
    {
        SingleKeyed,
        LeadingUnkeyedThenKeyed,
        AllKeyed,
        NoKeysAtAll
    }

    internal static class ProblemDetailsCodePairingFixture
    {
        internal const string SingleKey = "c-E404";

        internal const string LateKey = "c-E404";

        internal const string EarlyKey = "c-E500";

        internal const string SingleKeyedInfo = "SINGLE-KEYED-INFO";

        internal const string UnkeyedFirstInfo = "UNKEYED-FIRST-INFO";

        internal const string KeyedSecondInfo = "KEYED-SECOND-INFO";

        internal const string AllKeyedFirstInfo = "ALL-KEYED-FIRST-INFO";

        internal const string AllKeyedSecondInfo = "ALL-KEYED-SECOND-INFO";

        internal const string NoKeyFirstInfo = "NO-KEY-FIRST-INFO";

        internal const string NoKeySecondInfo = "NO-KEY-SECOND-INFO";

        internal static Result BuildNonGeneric(PairingScenario scenario)
        {
            var result = new Result { IsSuccess = false };
            Seed(result, scenario);

            return result;
        }

        internal static Result<string> BuildGeneric(PairingScenario scenario)
        {
            var result = new Result<string> { IsSuccess = false };
            Seed(result, scenario);

            return result;
        }

        internal static string InfoOfMessageOwningKey(IResult result, string key)
        {
            var owner = result.Messages.FirstOrDefault(message =>
                message != null && string.Equals(message.Key, key, StringComparison.Ordinal));

            return owner?.Message?.Info;
        }

        internal static string InfoOfFirstMessage(IResult result)
            => result.Messages.FirstOrDefault()?.Message?.Info;

        internal static string ReadCode(JObject problem) => problem.Value<string>("code");

        internal static bool HasCodeMember(JObject problem) => problem.Property("code") != null;

        internal static string Escape(string raw)
            => raw == null ? "<null>" : raw.Replace("{", "{{").Replace("}", "}}");

        internal static async Task<string> RenderObjectResultAsync(ObjectResult objectResult)
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

            await objectResult.ExecuteResultAsync(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));

            bodyStream.Position = 0;
            using var reader = new StreamReader(bodyStream, leaveOpen: true);

            return await reader.ReadToEndAsync();
        }

        private static void Seed<T>(Result<T> result, PairingScenario scenario)
        {
            switch (scenario)
            {
                case PairingScenario.SingleKeyed:
                    result.WithError(SingleKeyedInfo, SingleKey);

                    break;

                case PairingScenario.LeadingUnkeyedThenKeyed:
                    result.WithError(UnkeyedFirstInfo);
                    result.WithError(KeyedSecondInfo, LateKey);

                    break;

                case PairingScenario.AllKeyed:
                    result.WithError(AllKeyedFirstInfo, EarlyKey);
                    result.WithError(AllKeyedSecondInfo, LateKey);

                    break;

                case PairingScenario.NoKeysAtAll:
                    result.WithError(NoKeyFirstInfo);
                    result.WithError(NoKeySecondInfo);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario));
            }
        }
    }
}
