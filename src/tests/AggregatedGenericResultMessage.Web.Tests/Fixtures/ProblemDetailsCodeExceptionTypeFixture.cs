#region U S I N G

using RzR.ResultMessage.Abstractions.Models;

#endregion

namespace RzR.ResultMessage.Web.Tests.Fixtures
{
    internal static class ProblemDetailsCodeExceptionTypeFixture
    {
        internal const string ExceptionKey = "c-EXC500";

        internal const string SecondExceptionKey = "c-EXC501";

        internal const string ErrorKey = "c-ERR404";

        internal const string ErrorInfo = "ERROR-TYPED-INFO";

        internal static Result ExceptionFirstThenKeyedError()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new InvalidOperationException("exception-typed-text"), ExceptionKey);
            result.WithError(ErrorInfo, ErrorKey);

            return result;
        }

        internal static Result ExceptionFirstThenUnkeyedError()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new InvalidOperationException("exception-typed-text"), ExceptionKey);
            result.WithError(ErrorInfo);

            return result;
        }

        internal static Result AllExceptionTypedAndKeyed()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new InvalidOperationException("first-exception-text"), ExceptionKey);
            result.WithError(new InvalidOperationException("second-exception-text"), SecondExceptionKey);

            return result;
        }

        internal static Result AllExceptionTypedAndUnkeyed()
        {
            var result = new Result { IsSuccess = false };
            result.WithError(new InvalidOperationException("first-exception-text"), (string)null);
            result.WithError(new InvalidOperationException("second-exception-text"), (string)null);

            return result;
        }

        internal static Result ExceptionThenNullElement()
        {
            var seed = new Result { IsSuccess = false };
            seed.WithError(new InvalidOperationException("exception-typed-text"), ExceptionKey);

            var result = new Result { IsSuccess = false };
            result.Messages = new List<IMessageModel> { seed.Messages.First(), null };

            return result;
        }
    }
}
