// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 19:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 19:47
// ***********************************************************************
//  <copyright file="ResultToActionResultByCodeTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToActionResultByCodeTests
    {
        [TestMethod]
        public void AsActionResult_ResultSuccess_With200_ReturnsStatus200()
        {
            var sut = new Result { IsSuccess = true };

            var result = sut.AsActionResult(HttpStatusCode.OK);

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)result).StatusCode);
        }

        [TestMethod]
        public void AsActionResult_ResultSuccess_With201_ReturnsStatus201()
        {
            var sut = new Result { IsSuccess = true };

            var result = sut.AsActionResult(HttpStatusCode.Created);

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)result).StatusCode);
        }

        [TestMethod]
        public void AsActionResult_ResultFailure_WithErrorStatus_ReturnsThatStatusAndAllMessages()
        {
            var sut = new Result { IsSuccess = false }
                .WithError("e1")
                .WithError("e2");

            var result = sut.AsActionResult(HttpStatusCode.InternalServerError);

            // Failure path with an error status code currently returns BadRequestObjectResult
            // with overridden StatusCode and the full Messages collection.
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var messages = objectResult.Value as IEnumerable;
            Assert.IsNotNull(messages);
            Assert.AreEqual(2, messages.Cast<object>().Count());
        }

        [TestMethod]
        public void AsActionResult_ResultOfTSuccess_With200_ReturnsBody()
        {
            var sut = Result<int>.Success(7);

            var result = sut.AsActionResult(HttpStatusCode.OK);

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.AreEqual(7, objectResult.Value);
        }

        [TestMethod]
        public void AsActionResult_ResultOfTSuccess_With404_ReturnsAllMessages()
        {
            // Caller supplies an error status code for a successful result -> failure branch is taken.
            var sut = Result<int>.Success(7);

            var result = sut.AsActionResult(HttpStatusCode.NotFound);

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [TestMethod]
        public void AsIActionResult_IResultOfTFailure_With422_ReturnsThatStatusAndAllMessages()
        {
            IResult<string> sut =
                Result<string>.Failure("first")
                    .WithError("second")
                    .WithError("third");

            var result = sut.AsIActionResult(HttpStatusCode.UnprocessableEntity);

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, objectResult.StatusCode);
            var messages = objectResult.Value as IEnumerable;
            Assert.IsNotNull(messages);
            Assert.AreEqual(3, messages.Cast<object>().Count());
        }
    }
}