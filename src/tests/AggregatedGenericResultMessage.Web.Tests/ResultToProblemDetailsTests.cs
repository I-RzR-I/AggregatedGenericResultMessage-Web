// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author            : RzR
//  Created           : 24-05-2026 22:05
// 
//  Last Modified By : RzR
//  Last Modified On : 02-07-2026 19:48
//  ***********************************************************************
//  <copyright file="ResultToProblemDetailsTests.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

#region U S I N G

using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToProblemDetailsTests
    {
        [TestInitialize]
        public void Reset()
        {
            ResultStatusCodeMapper.Current = new DefaultResultStatusCodeMapper();
            ProblemDetailsResultFactory.Current = new DefaultProblemDetailsResultFactory();
        }

        [TestCleanup]
        public void Restore()
        {
            ResultStatusCodeMapper.Current = new DefaultResultStatusCodeMapper();
            ProblemDetailsResultFactory.Current = new DefaultProblemDetailsResultFactory();
        }

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
                additionalInformation: new Dictionary<string, object>
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

        [TestMethod]
        public void AsProblemDetails_NonGenericSuccess_NoStatusCode_ResolvesTo204_WithNullBody()
        {
            var sut = new Result { IsSuccess = true };

            var result = sut.AsProblemDetails();

            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public void AsProblemDetails_GenericSuccess_NoStatusCode_ResolvesTo200_WithResponseBody()
        {
            var sut = Result<int>.Success(42);

            var result = sut.AsProblemDetails();

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        public void AsProblemDetails_Failure_NoStatusCode_ResolvesTo400_WithProblemDetails()
        {
            var sut = new Result { IsSuccess = false }.WithError("mapped");

            var result = sut.AsProblemDetails();

            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(ResultMessageProblemDetails));
        }

        [TestMethod]
        public void AsProblemDetails_Failure_ExplicitStatusCode_OverridesMapper()
        {
            var sut = new Result { IsSuccess = false }.WithError("missing");

            var result = sut.AsProblemDetails(HttpStatusCode.NotFound);

            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public void AsProblemDetails_WithHttpContext_PopulatesTraceId()
        {
            var httpContext = new DefaultHttpContext { TraceIdentifier = "unit-trace-1" };
            var sut = new Result { IsSuccess = false }.WithError("nope");

            var result = sut.AsProblemDetails(HttpStatusCode.BadRequest, httpContext: httpContext);

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.AreEqual("unit-trace-1", problem.Extensions["traceId"]);
        }

        [TestMethod]
        public void AsProblemDetails_WithoutHttpContext_DoesNotPopulateTraceId()
        {
            var sut = new Result { IsSuccess = false }.WithError("nope");

            var result = sut.AsProblemDetails(HttpStatusCode.BadRequest);

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.IsFalse(problem.Extensions.ContainsKey("traceId"));
        }

        [TestMethod]
        public void AsProblemDetails_GenericSuccess_ExplicitCreatedStatusCode_ReturnsResponseBody_NotProblemDetails()
        {
            var sut = Result<int>.Success(99);

            var result = sut.AsProblemDetails(HttpStatusCode.Created);

            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual(99, result.Value);
            Assert.IsNotInstanceOfType(result.Value, typeof(ResultMessageProblemDetails));
        }
    }
}