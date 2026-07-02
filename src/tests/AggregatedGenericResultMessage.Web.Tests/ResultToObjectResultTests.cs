// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author            : RzR
//  Created           : 23-04-2026 21:04
// 
//  Last Modified By : RzR
//  Last Modified On : 02-07-2026 19:48
//  ***********************************************************************
//  <copyright file="ResultToObjectResultTests.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToObjectResultTests
    {
        [TestMethod]
        public void AsSuccessObjectResult_NonGenericResult_Returns204_WithNullBody()
        {
            var sut = new Result { IsSuccess = true };

            var result = sut.AsSuccessObjectResult();

            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public void AsSuccessObjectResult_NonGenericIResult_Returns204_WithNullBody()
        {
            IResult sut = new Result { IsSuccess = true };

            var result = sut.AsSuccessObjectResult();

            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public void AsSuccessObjectResult_ResultOfT_With200_ReturnsResponseAsBody()
        {
            var sut = Result<string>.Success("payload");

            var result = sut.AsSuccessObjectResult(HttpStatusCode.OK);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual("payload", result.Value);
        }

        [TestMethod]
        public void AsSuccessObjectResult_ResultOfT_With204_ReturnsNullBody()
        {
            var sut = Result<string>.Success("payload");

            var result = sut.AsSuccessObjectResult(HttpStatusCode.NoContent);

            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public void AsSuccessObjectResult_IResultOfT_With200_ReturnsResponseAsBody()
        {
            IResult<int> sut = Result<int>.Success(99);

            var result = sut.AsSuccessObjectResult(HttpStatusCode.OK);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(99, result.Value);
        }

        [TestMethod]
        public void AsSuccessObjectResult_ResultOfT_WithIntStatusCode_ReturnsBody()
        {
            var sut = Result<string>.Success("value");

            var result = sut.AsSuccessObjectResult(StatusCodes.Status201Created);

            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
            Assert.AreEqual("value", result.Value);
        }

        [TestMethod]
        public void AsSuccessObjectResult_ResultOfT_WithIntStatusCode200_ReturnsResponseAsBody()
        {
            var sut = Result<string>.Success("payload");

            var result = sut.AsSuccessObjectResult(StatusCodes.Status200OK);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual("payload", result.Value);
        }

        [TestMethod]
        public void AsSuccessObjectResult_IResultOfT_WithHttpStatusCode_NullResult_ReturnsNullBody()
        {
            IResult<int> sut = null;

            var result = sut.AsSuccessObjectResult(HttpStatusCode.OK);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsNull(result.Value);
        }
    }
}