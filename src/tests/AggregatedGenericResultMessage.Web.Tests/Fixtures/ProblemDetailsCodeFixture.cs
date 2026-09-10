#region U S I N G

using Newtonsoft.Json.Linq;
using RzR.ResultMessage.Models;

#endregion

namespace RzR.ResultMessage.Web.Tests.Fixtures
{
    internal static class ProblemDetailsCodeFixture
    {
        internal const string FirstKey = "c-E001";

        internal const string SecondKey = "c-E002";

        internal const string LateKey = "c-E777";

        internal static Result MultiMessageFailure()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new MessageDataModel("First validation error", "E001-First validation error"), FirstKey);
            result.WithError(new MessageDataModel("Second validation error", "E002-Second validation error"), SecondKey);
            result.WithError(new MessageDataModel("Third validation error", "E003-Third validation error"));

            return result;
        }

        internal static Result UnkeyedFailure()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new MessageDataModel("First validation error", "E001-First validation error"));
            result.WithError(new MessageDataModel("Second validation error", "E002-Second validation error"));

            return result;
        }

        internal static Result LeadingUnkeyedFailure()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new MessageDataModel("Unkeyed first error", "E000-Unkeyed first error"));
            result.WithError(new MessageDataModel("Keyed second error", "E777-Keyed second error"), LateKey);

            return result;
        }

        internal static string ReadCode(JObject problem) => problem.Value<string>("code");

        internal static bool HasCodeMember(JObject problem) => problem.Property("code") != null;

        internal static IReadOnlyList<string> ReadResultMessageKeys(JObject problem)
        {
            var messages = problem["extensions"]?["ResultMessages"] as JArray;

            return messages == null
                ? new List<string>()
                : messages
                    .Select(message => message.Value<string>("key") ?? message.Value<string>("Key"))
                    .ToList();
        }
    }
}
