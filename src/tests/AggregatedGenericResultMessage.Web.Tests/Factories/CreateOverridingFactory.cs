#region U S I N G

using RzR.ResultMessage.Web.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests.Factories
{
    internal sealed class CreateOverridingFactory : DefaultProblemDetailsResultFactory
    {
        internal const string InjectedCode = "<script>alert(1)</script>";

        public override ObjectResult Create(ResultProblemDetailsContext context)
            => new ObjectResult(new ResultMessageProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "wholly overridden",
                Code = InjectedCode
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
    }
}
