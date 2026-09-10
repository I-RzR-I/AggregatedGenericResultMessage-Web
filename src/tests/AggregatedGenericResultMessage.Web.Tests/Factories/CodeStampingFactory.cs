#region U S I N G

using RzR.ResultMessage.Web.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests.Factories
{
    internal sealed class CodeStampingFactory : DefaultProblemDetailsResultFactory
    {
        internal const string InjectedCode = "<script>alert(1)</script>";

        protected override void ApplyExtensions(ResultMessageProblemDetails problem,
            ResultProblemDetailsContext context)
        {
            base.ApplyExtensions(problem, context);
            problem.Code = InjectedCode;
        }
    }
}
