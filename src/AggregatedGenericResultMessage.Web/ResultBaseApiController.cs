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

using Microsoft.AspNetCore.Mvc;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Web.Extensions.Internal.DataType;

// ReSharper disable RedundantCast
#pragma warning disable IDE0004

#endregion

namespace RzR.ResultMessage.Web
{
    /// <summary>
    ///     Result controller base.
    ///     Return as JSON
    /// </summary>
    /// <remarks>
    ///     Derives from <see cref="ControllerBase" /> (not <see cref="T:Microsoft.AspNetCore.Mvc.Controller" />) so the
    ///     netstandard2.1 target only needs Microsoft.AspNetCore.Mvc.Core.
    /// </remarks>
    [ApiController]
    public abstract class ResultBaseApiController : ControllerBase
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
            => response.IsSuccess.IsTrue()
                ? AsJson(response.Response)
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
            => response.IsSuccess.IsTrue()
                ? response.Response.IsNull() ? (IActionResult)NoContent() : AsJson(response.Response)
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
            => response.IsSuccess.IsTrue()
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
            => response.IsSuccess.IsTrue()
                ? AsJson(response)
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
            => response.IsSuccess.IsTrue()
                ? response.Response.IsNull() ? (IActionResult)NoContent() : AsJson(response)
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
            => response.IsSuccess.IsTrue()
                ? (IActionResult)NoContent()
                : (IActionResult)BadRequest(response.Messages);

        /// <summary>
        ///     Wraps <paramref name="value" /> in a result that is always serialized as JSON.
        /// </summary>
        /// <param name="value">The payload to serialize.</param>
        /// <returns>An action result that always produces JSON.</returns>
        private static IActionResult AsJson(object value)
#if NETSTANDARD2_1
            => new OkObjectResult(value) { ContentTypes = { "application/json" } };
#else
            => new Microsoft.AspNetCore.Mvc.JsonResult(value);
#endif
    }
}