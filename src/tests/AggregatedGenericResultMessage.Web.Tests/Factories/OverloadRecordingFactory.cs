#region U S I N G

using RzR.ResultMessage.Web.Factories;

#endregion

namespace RzR.ResultMessage.Web.Tests.Factories
{
    internal sealed class OverloadRecordingFactory : DefaultProblemDetailsResultFactory
    {
        internal int CreateCallCount { get; private set; }

        internal bool? RecordedHasResponseBody { get; private set; }

        internal Type RecordedResultRuntimeType { get; private set; }

        public override ObjectResult Create(ResultProblemDetailsContext context)
        {
            CreateCallCount++;
            RecordedHasResponseBody = context.HasResponseBody;
            RecordedResultRuntimeType = context.Result?.GetType();

            return base.Create(context);
        }
    }
}
