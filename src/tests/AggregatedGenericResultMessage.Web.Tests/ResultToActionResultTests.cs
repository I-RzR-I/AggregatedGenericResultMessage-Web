// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 19:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 19:47
// ***********************************************************************
//  <copyright file="ResultToActionResultTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToActionResultTests
    {
        [TestMethod]
        public void AsActionResult_ResultSuccess_Returns204()
        {
            var sut = new Result { IsSuccess = true };

            var result = sut.AsActionResult();

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status204NoContent, ((StatusCodeResult)result).StatusCode);
        }

        [TestMethod]
        public void AsActionResult_ResultFailure_Returns400_WithAllMessages()
        {
            var sut = new Result { IsSuccess = false }
                .WithError("first")
                .WithError("second")
                .WithError("third");

            var result = sut.AsActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var messages = objectResult.Value as IEnumerable;
            Assert.IsNotNull(messages, "Failure body should be the full Messages collection.");
            Assert.AreEqual(3, messages.Cast<object>().Count());
        }

        [TestMethod]
        public void AsIActionResult_ResultSuccess_Returns204()
        {
            var sut = new Result { IsSuccess = true };

            var result = sut.AsIActionResult();

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status204NoContent, ((StatusCodeResult)result).StatusCode);
        }

        [TestMethod]
        public void AsActionResult_ResultOfTSuccess_Returns200_WithBody()
        {
            var sut = Result<int>.Success(42);

            var result = sut.AsActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.AreEqual(42, objectResult.Value);
        }

        [TestMethod]
        public void AsActionResult_ResultOfTFailure_Returns400_WithAllMessages()
        {
            var sut = Result<string>.Failure("first").WithError("second");

            var result = sut.AsActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var messages = objectResult.Value as IEnumerable;
            Assert.IsNotNull(messages);
            Assert.AreEqual(2, messages.Cast<object>().Count());
        }

        [TestMethod]
        public void AsIActionResult_ResultOfTSuccess_Returns200_WithBody()
        {
            var sut = Result<string>.Success("payload");

            var result = sut.AsIActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.AreEqual("payload", objectResult.Value);
        }

        [TestMethod]
        public void AsActionResult_IResultSuccess_Returns204()
        {
            IResult sut = new Result { IsSuccess = true };

            var result = sut.AsActionResult();

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status204NoContent, ((StatusCodeResult)result).StatusCode);
        }

        [TestMethod]
        public void AsActionResult_IResultFailure_Returns400_WithAllMessages()
        {
            IResult sut = new Result { IsSuccess = false }
                .WithError("e1")
                .WithError("e2");

            var result = sut.AsActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var messages = objectResult.Value as IEnumerable;
            Assert.IsNotNull(messages);
            Assert.AreEqual(2, messages.Cast<object>().Count());
        }

        [TestMethod]
        public void AsIActionResult_IResultOfTFailure_Returns400_WithAllMessages()
        {
            IResult<int> sut =
                Result<int>.Failure("e1").WithError("e2").WithError("e3");

            var result = sut.AsIActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var messages = objectResult.Value as IEnumerable;
            Assert.IsNotNull(messages);
            Assert.AreEqual(3, messages.Cast<object>().Count());
        }
    }
}