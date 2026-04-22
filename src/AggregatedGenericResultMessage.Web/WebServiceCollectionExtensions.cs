// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:25
// ***********************************************************************
//  <copyright file="WebServiceCollectionExtensions.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RzR.ResultMessage.Web.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Factories;
using RzR.ResultMessage.Web.Mappers;
using System;

#endregion

namespace RzR.ResultMessage.Web
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     <see cref="IServiceCollection" /> extensions for registering a custom
    ///     <see cref="IResultStatusCodeMapper" /> consumed by the static
    ///     <c>AsActionResult</c> / <c>AsEnvelopeActionResult</c> extensions.
    /// </summary>
    /// =================================================================================================
    public static class WebServiceCollectionExtensions
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <see cref="DefaultResultStatusCodeMapper" /> as the singleton
        ///     <see cref="IResultStatusCodeMapper" /> (idempotent) and wires it into
        ///     <see cref="ResultStatusCodeMapper.Current" />.
        /// </summary>
        /// <param name="services">The services to act on.</param>
        /// <returns>
        ///     An IServiceCollection.
        /// </returns>
        /// =================================================================================================
        public static IServiceCollection AddResultMessageWeb(this IServiceCollection services)
            => services.AddResultMessageWeb<DefaultResultStatusCodeMapper>();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TMapper" /> as the singleton
        ///     <see cref="IResultStatusCodeMapper" /> and immediately wires an instance into
        ///     <see cref="ResultStatusCodeMapper.Current" /> so the static extension methods
        ///     can use it without DI plumbing.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TMapper">Concrete mapper implementation.</typeparam>
        /// <param name="services">The services to act on.</param>
        /// <returns>
        ///     An IServiceCollection.
        /// </returns>
        /// =================================================================================================
        public static IServiceCollection AddResultMessageWeb<TMapper>(this IServiceCollection services)
            where TMapper : class, IResultStatusCodeMapper, new()
        {
            if (services.IsNull())
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IResultStatusCodeMapper, TMapper>();

            // Set the ambient mapper immediately so static extensions can use it without
            // requiring a service-provider build step. The DI registration above remains
            // available for consumers that want to resolve it from the container.
            ResultStatusCodeMapper.Current = new TMapper();

            return services;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers a caller-provided <paramref name="mapper" /> instance as the singleton
        ///     <see cref="IResultStatusCodeMapper" /> and wires it into
        ///     <see cref="ResultStatusCodeMapper.Current" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="services">The services to act on.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>
        ///     An IServiceCollection.
        /// </returns>
        /// =================================================================================================
        public static IServiceCollection AddResultMessageWeb(
            this IServiceCollection services, IResultStatusCodeMapper mapper)
        {
            if (services.IsNull())
                throw new ArgumentNullException(nameof(services));
            if (mapper.IsNull())
                throw new ArgumentNullException(nameof(mapper));

            services.TryAddSingleton(mapper);
            ResultStatusCodeMapper.Current = mapper;

            return services;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers <typeparamref name="TFactory" /> as the singleton
        ///     <see cref="IProblemDetailsResultFactory" /> and immediately wires an instance into
        ///     <see cref="ProblemDetailsResultFactory.Current" /> so the static
        ///     <c>AsProblemDetails</c> extensions can use it without DI plumbing. This lets callers
        ///     configure <c>type</c> / <c>title</c> / <c>instance</c> / extension defaults globally.
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <typeparam name="TFactory">Concrete factory implementation.</typeparam>
        /// <param name="services">The services to act on.</param>
        /// <returns>
        ///     An <see cref="IServiceCollection" /> for chaining.
        /// </returns>
        /// =================================================================================================
        public static IServiceCollection AddProblemDetailsResultFactory<TFactory>(this IServiceCollection services)
            where TFactory : class, IProblemDetailsResultFactory, new()
        {
            if (services.IsNull())
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IProblemDetailsResultFactory, TFactory>();
            ProblemDetailsResultFactory.Current = new TFactory();

            return services;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Registers a caller-provided <paramref name="factory" /> instance as the singleton
        ///     <see cref="IProblemDetailsResultFactory" /> and wires it into
        ///     <see cref="ProblemDetailsResultFactory.Current" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="services">The services to act on.</param>
        /// <param name="factory">The factory instance.</param>
        /// <returns>
        ///     An <see cref="IServiceCollection" /> for chaining.
        /// </returns>
        /// =================================================================================================
        public static IServiceCollection AddProblemDetailsResultFactory(
            this IServiceCollection services, IProblemDetailsResultFactory factory)
        {
            if (services.IsNull())
                throw new ArgumentNullException(nameof(services));
            if (factory.IsNull())
                throw new ArgumentNullException(nameof(factory));

            services.TryAddSingleton(factory);
            ProblemDetailsResultFactory.Current = factory;

            return services;
        }
    }
}