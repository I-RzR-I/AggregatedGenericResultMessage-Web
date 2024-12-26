// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-06 07:52
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-06 07:53
// ***********************************************************************
//  <copyright file="MessageStore.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace AggregatedGenericResultMessage.Web.Helpers.Store
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Messages to return from validation.
    /// </summary>
    /// =================================================================================================
    internal static class MessageStore
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Immutable) the HTTP status code not in successfully range.
        /// </summary>
        /// =================================================================================================
        internal const string HttpStatusCodeNotInSuccessfullyRange = "The current status code is not in the successful status range!";

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Immutable) the HTTP status code not in error range.
        /// </summary>
        /// =================================================================================================
        internal const string HttpStatusCodeNotInErrorRange = "The current status code is not in the Client/Server error status range!";
    }
}