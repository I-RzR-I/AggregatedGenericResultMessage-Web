namespace RzR.ResultMessage.Web.Tests.Controllers
{
    [Route("api/result-base")]
    public class ResultBaseApiTestController : ResultBaseApiController
    {
        [HttpGet("json-result")]
        public IActionResult GetJsonResult()
            => JsonResult(new Result<int> { IsSuccess = true, Response = 42 });

        [HttpGet("json-result-fail")]
        public IActionResult GetJsonResultFail()
            => JsonResult(new Result<int> { IsSuccess = false }.WithError("boom", "E001"));

        [HttpGet("json-result-null")]
        public IActionResult GetJsonResultNull()
            => JsonResult(new Result<string> { IsSuccess = true, Response = null });

        [HttpGet("json-result-null-check")]
        public IActionResult GetJsonResultNullCheck()
            => JsonResultWithNullCheck(new Result<string> { IsSuccess = true, Response = null });

        [HttpGet("json-whole")]
        public IActionResult GetJsonWhole()
            => JsonWholeResult(new Result<int> { IsSuccess = true, Response = 42 });

        [HttpGet("non-generic")]
        public IActionResult GetNonGeneric()
        {
            IResult result = new Result { IsSuccess = true };
            return JsonResult(result);
        }
    }
}
