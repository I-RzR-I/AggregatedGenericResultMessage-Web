#region U S I N G

using RzR.ResultMessage.Web.Exceptions;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests.Controllers
{
    [ApiController]
    [Route("api/code-exception")]
    public class ProblemDetailsCodeExceptionController : ControllerBase
    {
        [HttpGet("throw-keyed")]
        public IActionResult ThrowKeyed()
            => throw new WebResultException(ProblemDetailsCodeFixture.MultiMessageFailure());

        [HttpGet("throw-unkeyed")]
        public IActionResult ThrowUnkeyed()
            => throw new WebResultException(ProblemDetailsCodeFixture.UnkeyedFailure());

        [HttpGet("throw-exception-typed")]
        public IActionResult ThrowExceptionTyped()
            => throw new WebResultException(
                ProblemDetailsCodeExceptionTypeFixture.ExceptionFirstThenKeyedError());

        [HttpGet("throw-all-exception-typed")]
        public IActionResult ThrowAllExceptionTyped()
            => throw new WebResultException(
                ProblemDetailsCodeExceptionTypeFixture.AllExceptionTypedAndKeyed());
    }
}
