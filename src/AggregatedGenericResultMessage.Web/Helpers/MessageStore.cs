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

namespace AggregatedGenericResultMessage.Web.Helpers
{
    /// <summary>
    ///     Messages to return from validation
    /// </summary>
    internal static class MessageStore
    {
        internal const string HttpStatusCodeNotInSuccessfullyRange = "The current status code is not in the successful status range!";
        internal const string HttpStatusCodeNotInErrorRange = "The current status code is not in the Client/Server error status range!";
    }
}