#region U S I N G

using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsCodeContractBoundaryTests
    {
        private const string Because =
            "The pre-RFC7807 families are a separate ratified contract and must not emit a problem body or a top-level code.";

        [TestInitialize]
        public void Reset()
        {
            ResultStatusCodeMapper.Current = new DefaultResultStatusCodeMapper();
            ProblemDetailsResultFactory.Current = new DefaultProblemDetailsResultFactory();
        }

        [TestMethod]
        public void AsIActionResult_KeyedFailure_ProducesNoProblemBody_Test()
        {
            var result = ProblemDetailsCodeFixture.MultiMessageFailure().AsIActionResult();

            var objectResult = (ObjectResult)result;

            Assert.IsNotInstanceOfType(objectResult.Value, typeof(ResultMessageProblemDetails), Because);
            Assert.IsInstanceOfType(objectResult.Value, typeof(IEnumerable));
        }

        [TestMethod]
        public void AsIActionResultWithStatus_KeyedFailure_ProducesNoProblemBody_Test()
        {
            var result = ProblemDetailsCodeFixture.MultiMessageFailure().AsIActionResult(HttpStatusCode.BadRequest);

            var objectResult = (ObjectResult)result;

            Assert.IsNotInstanceOfType(objectResult.Value, typeof(ResultMessageProblemDetails), Because);
            Assert.IsInstanceOfType(objectResult.Value, typeof(IEnumerable));
        }

        [TestMethod]
        public void AsEnvelopeIActionResult_KeyedFailure_ProducesNoProblemBody_Test()
        {
            var result = ProblemDetailsCodeFixture.MultiMessageFailure().AsEnvelopeIActionResult();

            var objectResult = (ObjectResult)result;

            Assert.IsNotInstanceOfType(objectResult.Value, typeof(ResultMessageProblemDetails), Because);
            Assert.IsInstanceOfType(objectResult.Value, typeof(IResult));
        }

        [TestMethod]
        public void JsonResult_KeyedFailure_ProducesNoProblemBody_Test()
        {
            var result = new BoundaryProbeController().InvokeJsonResult(ProblemDetailsCodeFixture.MultiMessageFailure());

            var objectResult = (ObjectResult)result;

            Assert.IsNotInstanceOfType(objectResult.Value, typeof(ResultMessageProblemDetails), Because);
            Assert.IsInstanceOfType(objectResult.Value, typeof(IEnumerable));
        }

        [TestMethod]
        public void AsProblemDetails_KeyedFailure_StillProducesAProblemBodyWithCode_Test()
        {
            var result = ProblemDetailsCodeFixture.MultiMessageFailure().AsProblemDetails(HttpStatusCode.BadRequest);

            var problem = (ResultMessageProblemDetails)result.Value;

            Assert.AreEqual(ProblemDetailsCodeFixture.FirstKey, problem.Code,
                "The problem-details family is the other side of the boundary and must keep emitting the code.");
        }

        private sealed class BoundaryProbeController : ResultBaseApiController
        {
            public IActionResult InvokeJsonResult(IResult response) => JsonResult(response);
        }
    }
}
