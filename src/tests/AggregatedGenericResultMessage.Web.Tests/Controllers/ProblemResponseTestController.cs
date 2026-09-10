#region U S A G E S

using RzR.ResultMessage.Web.Extensions.Unified;

#endregion

namespace RzR.ResultMessage.Web.Tests.Controllers
{
    [ApiController]
    [Route("api/problem-response")]
    public class ProblemResponseTestController : ControllerBase
    {
        [HttpGet("ok")]
        public IActionResult Ok42()
        {
            return Result<int>.Success(42).ToProblemResponse();
        }

        [HttpGet("fail")]
        public IActionResult Fail()
        {
            return new Result { IsSuccess = false }.WithError("invalid").ToProblemResponse();
        }

        [HttpGet("nocontent")]
        public IActionResult NoBody()
        {
            return new Result { IsSuccess = true }.ToProblemResponse();
        }
    }
}