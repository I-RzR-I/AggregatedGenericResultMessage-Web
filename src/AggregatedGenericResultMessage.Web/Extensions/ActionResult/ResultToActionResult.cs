// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-07 00:42
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-08 18:20
// ***********************************************************************
//  <copyright file="ResultToActionResult.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Linq;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.Internal.DataType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.ActionResult
{
    /// <summary>
    ///     Result to Action/Object Result
    ///     Result -> ActionResult
    /// </summary>
    public static partial class ToActionResult
    {
        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 204 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this Result result)
            => result.IsSuccess.IsTrue()
                ? (Microsoft.AspNetCore.Mvc.ActionResult)new StatusCodeResult(StatusCodes.Status204NoContent)
                : new ObjectResult(result.GetFirstMessage()) { StatusCode = StatusCodes.Status400BadRequest };

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 204 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult(this Result result)
            => result.IsSuccess.IsTrue()
                ? (IActionResult)new StatusCodeResult(StatusCodes.Status204NoContent)
                : new ObjectResult(result.GetFirstMessage()) { StatusCode = StatusCodes.Status400BadRequest };

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 200 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this Result<T> result)
            => result.IsSuccess.IsTrue()
                ? new ObjectResult(result.Response) { StatusCode = StatusCodes.Status200OK }
                : new ObjectResult(result.GetFirstMessage()) { StatusCode = StatusCodes.Status400BadRequest };

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 200 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult<T>(this Result<T> result)
            => result.IsSuccess.IsTrue()
                ? new ObjectResult(result.Response) { StatusCode = StatusCodes.Status200OK }
                : new ObjectResult(result.GetFirstMessage()) { StatusCode = StatusCodes.Status400BadRequest };

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 204 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult(this IResult result)
            => result.IsSuccess.IsTrue()
                ? (Microsoft.AspNetCore.Mvc.ActionResult)new StatusCodeResult(StatusCodes.Status204NoContent)
                : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = StatusCodes.Status400BadRequest };

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 204 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult(this IResult result)
            => result.IsSuccess.IsTrue()
                ? (IActionResult)new StatusCodeResult(StatusCodes.Status204NoContent)
                : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = StatusCodes.Status400BadRequest };

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 200 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static Microsoft.AspNetCore.Mvc.ActionResult AsActionResult<T>(this IResult<T> result)
            => result.IsSuccess.IsTrue()
                ? new ObjectResult(result.Response) { StatusCode = StatusCodes.Status200OK }
                : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = StatusCodes.Status400BadRequest };

        /// <summary>
        ///     Result to ActionResult
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>
        ///     Return api response in JSON format.
        ///     Status code 200 if IsSuccess is true.
        ///     Status code 400 with error if IsSuccess is false.
        /// </returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static IActionResult AsIActionResult<T>(this IResult<T> result)
            => result.IsSuccess.IsTrue()
                ? new ObjectResult(result.Response) { StatusCode = StatusCodes.Status200OK }
                : new ObjectResult(result.Messages.FirstOrDefault()?.Message) { StatusCode = StatusCodes.Status400BadRequest };
    }
}