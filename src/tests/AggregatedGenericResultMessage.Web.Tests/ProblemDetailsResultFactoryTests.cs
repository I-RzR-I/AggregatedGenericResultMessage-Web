// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web.Tests
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 21:28
// ***********************************************************************
//  <copyright file="ProblemDetailsResultFactoryTests.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using RzR.ResultMessage.Web.Abstractions;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Tests.Factories;
using RzR.ResultMessage.Web.WebDependencyInjection;

#endregion

namespace RzR.ResultMessage.Web.Tests
{
    [TestClass]
    public class ProblemDetailsResultFactoryTests
    {
        [TestInitialize]
        public void ResetFactory()
        {
            ProblemDetailsResultFactory.Current = new DefaultProblemDetailsResultFactory();
        }

        [TestCleanup]
        public void RestoreDefault()
        {
            ProblemDetailsResultFactory.Current = new DefaultProblemDetailsResultFactory();
        }

        [TestMethod]
        public void Default_FailureProblem_KeepsBaselineShape()
        {
            var sut = new Result { IsSuccess = false }.WithError("oops");

            var result = sut.AsProblemDetails(HttpStatusCode.BadRequest);

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.AreEqual(StatusCodes.Status400BadRequest, problem.Status);
            StringAssert.Contains(problem.Type, "rfc");
            Assert.IsTrue(problem.Extensions.ContainsKey("ResultMessages"));
        }

        [TestMethod]
        public void CustomFactory_AppliesGlobalDefaults()
        {
            ProblemDetailsResultFactory.Current = new BrandedFactory();

            var sut = new Result { IsSuccess = false }.WithError("oops");

            var result = sut.AsProblemDetails(HttpStatusCode.BadRequest);

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.AreEqual("https://errors.example.com/400", problem.Type);
            Assert.AreEqual("Bad request occurred", problem.Title);
            Assert.AreEqual("/api/orders", problem.Instance);
            Assert.IsTrue(problem.Extensions.ContainsKey("traceId"));
            Assert.AreEqual("trace-123", problem.Extensions["traceId"]);
        }

        [TestMethod]
        public void CustomFactory_PerCallOverridesWinOverDefaults()
        {
            ProblemDetailsResultFactory.Current = new BrandedFactory();

            var sut = new Result { IsSuccess = false }.WithError("oops");

            var result = sut.AsProblemDetails(
                HttpStatusCode.UnprocessableEntity,
                "Validation failed",
                "see body",
                "/api/orders/42");

            var problem = (ResultMessageProblemDetails)result.Value!;
            Assert.AreEqual("Validation failed", problem.Title);
            Assert.AreEqual("see body", problem.Detail);
            Assert.AreEqual("/api/orders/42", problem.Instance);

            // Type still resolved by the factory.
            Assert.AreEqual("https://errors.example.com/422", problem.Type);
        }

        [TestMethod]
        public void CustomFactory_GenericSuccess_StillReturnsResponseBody()
        {
            ProblemDetailsResultFactory.Current = new BrandedFactory();

            var sut = Result<int>.Success(42);

            var result = sut.AsProblemDetails(HttpStatusCode.OK);

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        public void Current_AssigningNull_RestoresDefault()
        {
            ProblemDetailsResultFactory.Current = new BrandedFactory();
            ProblemDetailsResultFactory.Current = null!;

            Assert.IsInstanceOfType(ProblemDetailsResultFactory.Current, typeof(DefaultProblemDetailsResultFactory));
        }

        [TestMethod]
        public void AddProblemDetailsResultFactory_GenericRegistersAndSetsCurrent()
        {
            var services = new ServiceCollection();

            services.AddProblemDetailsResultFactory<BrandedFactory>();

            using var provider = services.BuildServiceProvider();
            var resolved = provider.GetRequiredService<IProblemDetailsResultFactory>();

            Assert.IsInstanceOfType(resolved, typeof(BrandedFactory));
            Assert.IsInstanceOfType(ProblemDetailsResultFactory.Current, typeof(BrandedFactory));
        }

        [TestMethod]
        public void AddProblemDetailsResultFactory_InstanceRegistersAndSetsCurrent()
        {
            var services = new ServiceCollection();
            var factory = new BrandedFactory();

            services.AddProblemDetailsResultFactory(factory);

            using var provider = services.BuildServiceProvider();
            var resolved = provider.GetRequiredService<IProblemDetailsResultFactory>();

            Assert.AreSame(factory, resolved);
            Assert.AreSame(factory, ProblemDetailsResultFactory.Current);
        }
    }
}