#region U S I N G

using RzR.ResultMessage.Web.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests.Factories
{
    internal sealed class HostileCodeFactory : DefaultProblemDetailsResultFactory
    {
        internal string CodeToReturn { get; set; }

        protected override string ResolveCode(ResultProblemDetailsContext context) => CodeToReturn;
    }
}
