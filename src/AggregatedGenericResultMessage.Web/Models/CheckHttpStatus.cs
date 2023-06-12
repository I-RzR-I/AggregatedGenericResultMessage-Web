// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-06 08:39
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-06 08:39
// ***********************************************************************
//  <copyright file="CheckHttpStatus.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace AggregatedGenericResultMessage.Web.Models
{
    /// <summary>
    ///     Check HTTP status code model
    /// </summary>
    internal class CheckHttpStatus
    {
        /// <summary>
        ///     Is success HTTP status code
        /// </summary>
        internal bool IsSuccess { get; set; }
        
        /// <summary>
        ///     Is error HTTP status code
        /// </summary>
        internal bool IsError { get; set; }
    }
}