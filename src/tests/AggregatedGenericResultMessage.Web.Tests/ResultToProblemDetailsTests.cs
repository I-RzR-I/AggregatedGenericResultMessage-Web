// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 19:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 19:48
// ***********************************************************************
//  <copyright file="ResultToProblemDetailsTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToProblemDetailsTests
    {
        [TestMethod]
        public void AsProblemDetails_NonGenericFailure_Returns400_WithProblemDetails()
        {
            var sut = new Result { IsSuccess = false }
                .WithError("first")
                .WithError("second");

            var result = sut.AsProblemDetails(HttpStatusCode.BadRequest);

            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            var problem = result.Value as ResultMessageProblemDetails;
            Assert.IsNotNull(problem);
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Status);
            Assert.IsFalse(string.IsNullOrEmpty(problem.Type));
            StringAssert.Contains(problem.Type, "rfc");
        }

        [TestMethod]
        public void AsProblemDetails_NonGenericFailure_WithCustomMessageAndDetail_PopulatesProblem()
        {
            var sut = new Result { IsSuccess = false }.WithError("oops");

            var result = sut.AsProblemDetails(
                HttpStatusCode.UnprocessableEntity,
                "Custom title",
                "Custom detail",
                "/api/things/42");

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, problem.Status);
            Assert.AreEqual("Custom title", problem.Title);
            Assert.AreEqual("Custom detail", problem.Detail);
            Assert.AreEqual("/api/things/42", problem.Instance);
        }

        [TestMethod]
        public void AsProblemDetails_GenericSuccess_With200_ReturnsResponseBody()
        {
            var sut = Result<int>.Success(42);

            var result = sut.AsProblemDetails(HttpStatusCode.OK);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        public void AsProblemDetails_GenericFailure_PopulatesAdditionalInformation()
        {
            var sut = Result<string>.Failure("e1");

            var result = sut.AsProblemDetails(
                HttpStatusCode.BadRequest,
                additionInformation: new Dictionary<string, object>
                {
                    ["traceId"] = "abc-123"
                });

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.IsTrue(problem.Extensions.ContainsKey("traceId"));
            Assert.AreEqual("abc-123", problem.Extensions["traceId"]);
            Assert.IsTrue(problem.Extensions.ContainsKey("ResultMessages"));
        }

        [TestMethod]
        public void AsProblemDetails_UnknownStatusCode_FallsBackToAboutBlank()
        {
            // Status code 418 has no entry in the RFC dictionary; lookup should default to "about:blank".
            var sut = new Result { IsSuccess = false }.WithError("teapot");

            var result = sut.AsProblemDetails((HttpStatusCode)418);

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.AreEqual(418, problem.Status);
            Assert.AreEqual("about:blank", problem.Type);
        }
    }
}