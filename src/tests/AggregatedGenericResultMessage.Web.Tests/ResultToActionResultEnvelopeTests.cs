// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 19:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 19:47
// ***********************************************************************
//  <copyright file="ResultToActionResultEnvelopeTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultToActionResultEnvelopeTests
    {
        [TestMethod]
        public void AsEnvelopeActionResult_IResultSuccess_DefaultsTo200_WithFullEnvelope()
        {
            IResult sut = new Result { IsSuccess = true };

            var result = sut.AsEnvelopeActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.AreSame(sut, objectResult.Value);
        }

        [TestMethod]
        public void AsEnvelopeActionResult_IResultFailure_DefaultsTo400_WithFullEnvelope()
        {
            IResult sut = new Result { IsSuccess = false }.WithError("oops");

            var result = sut.AsEnvelopeActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.AreSame(sut, objectResult.Value);
        }

        [TestMethod]
        public void AsEnvelopeActionResult_GenericSuccess_With201_ReturnsEnvelope()
        {
            IResult<int> sut = Result<int>.Success(123);

            var result = sut.AsEnvelopeActionResult(HttpStatusCode.Created);

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status201Created, objectResult.StatusCode);
            Assert.AreSame(sut, objectResult.Value);
        }

        [TestMethod]
        public void AsEnvelopeIActionResult_GenericFailure_With422_ReturnsEnvelopeWithMessages()
        {
            IResult<int> sut =
                Result<int>.Failure("e1").WithError("e2");

            var result = sut.AsEnvelopeIActionResult(HttpStatusCode.UnprocessableEntity);

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, objectResult.StatusCode);
            var envelope = objectResult.Value as IResult;
            Assert.IsNotNull(envelope);
            Assert.IsFalse(envelope.IsSuccess);
            Assert.AreEqual(2, envelope.Messages.Count());
        }

        [TestMethod]
        public void AsEnvelopeActionResult_InvalidStatusCode_FallsBackTo400()
        {
            // 999 is not a valid HTTP status; CheckStatusCode considers it not successful.
            IResult sut = new Result { IsSuccess = true };

            var result = sut.AsEnvelopeActionResult((HttpStatusCode)999);

            var objectResult = (ObjectResult)result;
            // Implementation falls back to 400 BadRequest for invalid codes while still echoing the envelope.
            Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.AreSame(sut, objectResult.Value);
        }
    }
}