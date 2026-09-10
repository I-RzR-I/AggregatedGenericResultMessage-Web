#region U S I N G

using RzR.ResultMessage.Web.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests.Factories
{
    internal sealed class WellFormedCodeStampingFactory : DefaultProblemDetailsResultFactory
    {
        internal const string InjectedCode = "c-STAMPED-FROM-HOOK";

        protected override void ApplyExtensions(ResultMessageProblemDetails problem,
            ResultProblemDetailsContext context)
        {
            base.ApplyExtensions(problem, context);
            problem.Code = InjectedCode;
        }
    }
}
