// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:30
// ***********************************************************************
//  <copyright file="ResultStatusCodeMapperTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.ResultMessage.Web.Abstractions;
using RzR.ResultMessage.Web.Mappers;
using RzR.ResultMessage.Web.Tests.Mappers;
using RzR.ResultMessage.Web.WebDependencyInjection;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ResultStatusCodeMapperTests
    {
        [TestInitialize]
        public void ResetMapper()
        {
            ResultStatusCodeMapper.Current = new DefaultResultStatusCodeMapper();
        }

        [TestCleanup]
        public void RestoreDefault()
        {
            ResultStatusCodeMapper.Current = new DefaultResultStatusCodeMapper();
        }

        [TestMethod]
        public void Default_NonGenericSuccess_Returns204()
        {
            var sut = new Result { IsSuccess = true };

            var result = sut.AsActionResult();

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status204NoContent, ((StatusCodeResult)result).StatusCode);
        }

        [TestMethod]
        public void Default_GenericSuccess_Returns200_WithResponseBody()
        {
            var sut = Result<int>.Success(42);

            var result = sut.AsActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.AreEqual(42, objectResult.Value);
        }

        [TestMethod]
        public void CustomMapper_FailureReturns404_WhenInspectingMessages()
        {
            ResultStatusCodeMapper.Current = new NotFoundOnMissingMapper();

            var sut = new Result { IsSuccess = false }
                .WithError("missing")
                .WithError("other detail");

            var result = sut.AsActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status404NotFound, objectResult.StatusCode);

            // Body is still the full Messages collection.
            var messages = objectResult.Value as IEnumerable;
            Assert.IsNotNull(messages);
            Assert.AreEqual(2, messages.Cast<object>().Count());
        }

        [TestMethod]
        public void CustomMapper_FailureReturns409_WhenInspectingMessages()
        {
            ResultStatusCodeMapper.Current = new ConflictOnDuplicateMapper();

            var sut = new Result { IsSuccess = false }.WithError("duplicate key");

            var result = sut.AsActionResult();

            Assert.AreEqual(StatusCodes.Status409Conflict, ((ObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void CustomMapper_AppliesToEnvelopeDefault()
        {
            ResultStatusCodeMapper.Current = new ConflictOnDuplicateMapper();

            IResult sut = new Result { IsSuccess = false }.WithError("duplicate key");

            var result = sut.AsEnvelopeIActionResult();

            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status409Conflict, objectResult.StatusCode);
            Assert.AreSame(sut, objectResult.Value);
        }

        [TestMethod]
        public void Current_AssigningNull_RestoresDefault()
        {
            ResultStatusCodeMapper.Current = new ConflictOnDuplicateMapper();
            ResultStatusCodeMapper.Current = null!;

            Assert.IsInstanceOfType(ResultStatusCodeMapper.Current, typeof(DefaultResultStatusCodeMapper));
        }

        [TestMethod]
        public void AddWebResultMessageMapper_GenericRegistersAndSetsCurrent()
        {
            var services = new ServiceCollection();

            services.AddWebResultMessageMapper<NotFoundOnMissingMapper>();

            using var provider = services.BuildServiceProvider();
            var resolved = provider.GetRequiredService<IResultStatusCodeMapper>();

            Assert.IsInstanceOfType(resolved, typeof(NotFoundOnMissingMapper));
            Assert.IsInstanceOfType(ResultStatusCodeMapper.Current, typeof(NotFoundOnMissingMapper));
        }

        [TestMethod]
        public void AddWebResultMessageMapper_InstanceOverloadRegistersAndSetsCurrent()
        {
            var services = new ServiceCollection();
            var mapper = new ConflictOnDuplicateMapper();

            services.AddWebResultMessageMapper(mapper);

            using var provider = services.BuildServiceProvider();
            var resolved = provider.GetRequiredService<IResultStatusCodeMapper>();

            Assert.AreSame(mapper, resolved);
            Assert.AreSame(mapper, ResultStatusCodeMapper.Current);
        }
    }
}