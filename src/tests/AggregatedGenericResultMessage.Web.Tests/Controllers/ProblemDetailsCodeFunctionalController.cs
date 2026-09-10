#region U S I N G

using RzR.ResultMessage.Web.Extensions.Unified;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests.Controllers
{
    [ApiController]
    [Route("api/code-functional")]
    public class ProblemDetailsCodeFunctionalController : ControllerBase
    {
        [HttpGet("multi-message-fail")]
        public IActionResult MultiMessageFail()
            => ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse();

        [HttpGet("unkeyed-fail")]
        public IActionResult UnkeyedFail()
            => ProblemDetailsCodeFixture.UnkeyedFailure().ToProblemResponse();

        [HttpGet("leading-unkeyed-fail")]
        public IActionResult LeadingUnkeyedFail()
            => ProblemDetailsCodeFixture.LeadingUnkeyedFailure().ToProblemResponse();

        [HttpGet("generic-success")]
        public IActionResult GenericSuccess()
            => Result<int>.Success(42).ToProblemResponse();

        [HttpGet("non-generic-success")]
        public IActionResult NonGenericSuccess()
            => new Result { IsSuccess = true }.ToProblemResponse();
    }
}
