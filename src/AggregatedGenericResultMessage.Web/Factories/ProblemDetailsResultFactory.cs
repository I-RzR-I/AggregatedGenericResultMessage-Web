// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 21:00
// ***********************************************************************
//  <copyright file="ProblemDetailsResultFactory.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Web.Abstractions;

#endregion

namespace RzR.ResultMessage.Web.Factories
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Holds the ambient <see cref="IProblemDetailsResultFactory" /> consulted by the static
    ///     <c>AsProblemDetails</c> extensions. Defaults to
    ///     <see cref="DefaultProblemDetailsResultFactory" />. Set directly for non-DI scenarios or
    ///     implicitly via <c>AddProblemDetailsResultFactory&lt;TFactory&gt;()</c>.
    /// </summary>
    /// =================================================================================================
    public static class ProblemDetailsResultFactory
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     The current.
        /// </summary>
        /// =================================================================================================
        private static IProblemDetailsResultFactory _current = new DefaultProblemDetailsResultFactory();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Currently active factory. Never <see langword="null" /> — assigning <c>null</c>
        ///     restores the default factory.
        /// </summary>
        /// <value>
        ///     The current.
        /// </value>
        /// =================================================================================================
        public static IProblemDetailsResultFactory Current
        {
            get => _current;
            set => _current = value ?? new DefaultProblemDetailsResultFactory();
        }
    }
}