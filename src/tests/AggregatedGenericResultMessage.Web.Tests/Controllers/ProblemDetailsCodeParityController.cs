#region U S I N G

using RzR.ResultMessage.Web.Extensions.Unified;
using RzR.ResultMessage.Web.Tests.Fixtures;

#endregion

namespace RzR.ResultMessage.Web.Tests.Controllers
{
    [ApiController]
    [Route("api/code-parity")]
    public class ProblemDetailsCodeParityController : ControllerBase
    {
        [HttpGet("as-problem-details")]
        public IActionResult AsProblemDetailsSurface()
            => ProblemDetailsCodeFixture.MultiMessageFailure().AsProblemDetails(HttpStatusCode.BadRequest);

        [HttpGet("to-problem-response")]
        public IActionResult ToProblemResponseSurface()
            => ProblemDetailsCodeFixture.MultiMessageFailure().ToProblemResponse(HttpStatusCode.BadRequest);

        [HttpGet("as-problem-details-unkeyed")]
        public IActionResult AsProblemDetailsUnkeyedSurface()
            => ProblemDetailsCodeFixture.UnkeyedFailure().AsProblemDetails(HttpStatusCode.BadRequest);

        [HttpGet("to-problem-response-unkeyed")]
        public IActionResult ToProblemResponseUnkeyedSurface()
            => ProblemDetailsCodeFixture.UnkeyedFailure().ToProblemResponse(HttpStatusCode.BadRequest);
    }
}
