// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2026-04-22 20:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2026-04-22 20:19
// ***********************************************************************
//  <copyright file="ResultStatusCodeMapper.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using RzR.ResultMessage.Web.Abstractions;

#endregion

namespace RzR.ResultMessage.Web.Mappers
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Holds the ambient <see cref="IResultStatusCodeMapper" /> consulted by the static
    ///     <c>AsActionResult</c> / <c>AsIActionResult</c> / <c>AsEnvelopeActionResult</c>
    ///     extensions. Defaults to <see cref="DefaultResultStatusCodeMapper" />. Set directly for
    ///     non-DI scenarios or implicitly via <c>AddResultMessageWeb&lt;TMapper&gt;()</c>.
    /// </summary>
    /// =================================================================================================
    public static class ResultStatusCodeMapper
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     The current.
        /// </summary>
        /// =================================================================================================
        private static IResultStatusCodeMapper _current = new DefaultResultStatusCodeMapper();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Currently active mapper. Never <see langword="null" /> — assigning <c>null</c>
        ///     restores the default mapper.
        /// </summary>
        /// <value>
        ///     The current.
        /// </value>
        /// =================================================================================================
        public static IResultStatusCodeMapper Current
        {
            get => _current;
            set => _current = value ?? new DefaultResultStatusCodeMapper();
        }
    }
}