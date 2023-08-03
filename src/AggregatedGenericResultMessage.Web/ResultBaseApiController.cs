// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-05 23:06
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-09 08:51
// ***********************************************************************
//  <copyright file="ResultBaseApiController.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.Internal;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable RedundantCast
#pragma warning disable IDE0004

#endregion

namespace AggregatedGenericResultMessage.Web
{
    /// <summary>
    ///     Result controller base.
    ///     Return as JSON
    /// </summary>
    /// <remarks></remarks>
    [ApiController]
    public abstract class ResultBaseApiController : Controller
    {
        /// <summary>
        ///     Return API response in JSON format.
        /// </summary>
        /// <typeparam name="T">Result response type</typeparam>
        /// <param name="response">Result response</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 200 with result value if IsSuccess is true.
        ///     Status code 400 with errors collection if IsSuccess is false.
        /// </returns>
        protected virtual IActionResult JsonResult<T>(IResult<T> response)
            => response.IsSuccess
                ? (IActionResult)Json(response.Response)
                : BadRequest(response.Messages);

        /// <summary>
        ///     Return API response in JSON format.
        /// </summary>
        /// <typeparam name="T">Result response type</typeparam>
        /// <param name="response">Result response</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 200 with result value if IsSuccess is true.
        ///     Status code 204 in case when Response is null.
        ///     Status code 400 with errors collection if IsSuccess is false.
        /// </returns>
        protected virtual IActionResult JsonResultWithNullCheck<T>(IResult<T> response)
            => response.IsSuccess
                ? response.Response.IsNull() ? (IActionResult)NoContent() : Json(response.Response)
                : BadRequest(response.Messages);

        /// <summary>
        ///     Return API response in JSON format.
        /// </summary>
        /// <param name="response">Result response</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 204 if IsSuccess is true.
        ///     Status code 400 with errors collection if IsSuccess is false.
        /// </returns>
        protected virtual IActionResult JsonResult(IResult response)
            => response.IsSuccess
                ? (IActionResult)NoContent()
                : BadRequest(response.Messages);

        /// <summary>
        ///     Return API response in JSON format.
        /// </summary>
        /// <param name="response">Result response</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 200 with result value if IsSuccess is true.
        ///     Status code 400 with errors collection if IsSuccess is false.
        /// </returns>
        /// <typeparam name="T">Result response type</typeparam>
        /// <remarks></remarks>
        protected virtual IActionResult JsonWholeResult<T>(IResult<T> response)
            => response.IsSuccess
                ? (IActionResult)Json(response)
                : (IActionResult)BadRequest(response.Messages);

        /// <summary>
        ///     Return API response in JSON format.
        /// </summary>
        /// <param name="response">Result response</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 204 if IsSuccess is true.
        ///     Status code 400 with errors collection if IsSuccess is false.
        /// </returns>
        /// <typeparam name="T">Result response type</typeparam>
        /// <remarks></remarks>
        protected virtual IActionResult JsonWholeResultWithNullCheck<T>(IResult<T> response)
            => response.IsSuccess
                ? response.Response.IsNull() ? (IActionResult)NoContent() : Json(response)
                : (IActionResult)BadRequest(response.Messages);

        /// <summary>
        ///     Return API response in JSON format.
        /// </summary>
        /// <param name="response">Result response</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 204 if IsSuccess is true.
        ///     Status code 400 with errors collection if IsSuccess is false.
        /// </returns>
        /// <remarks></remarks>
        protected virtual IActionResult JsonWholeResult(IResult response)
            => response.IsSuccess
                ? (IActionResult)NoContent()
                : (IActionResult)BadRequest(response.Messages);
    }
}