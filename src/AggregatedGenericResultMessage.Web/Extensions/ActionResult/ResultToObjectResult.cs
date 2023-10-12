// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-07 17:58
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-08 18:47
// ***********************************************************************
//  <copyright file="ResultToObjectResult.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.Internal.DataType;
using AggregatedGenericResultMessage.Web.Extensions.Internal.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable IDE0060
#pragma warning disable IDE0090
#pragma warning disable IDE0090

#endregion

namespace AggregatedGenericResultMessage.Web.Extensions.ActionResult
{
    /// <summary>
    ///     Result to Action/Object Result
    /// </summary>
    public static partial class ToActionResult
    {
        /// <summary>
        ///     Result to ObjectResult (as success result)
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>Return <see cref="ObjectResult"/> from common <see cref="Result"/> as success (HTTP status = 204)</returns>
        /// <remarks></remarks>
        public static ObjectResult AsSuccessObjectResult(this Result result)
            => new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent };

        /// <summary>
        ///     Result to ObjectResult (as success result)
        /// </summary>
        /// <param name="result">Common result</param>
        /// <returns>Return <see cref="ObjectResult"/> from common <see cref="IResult"/> as success (HTTP status 204)</returns>
        /// <remarks></remarks>
        public static ObjectResult AsSuccessObjectResult(this IResult result)
            => new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent };

        /// <summary>
        ///     Result to ObjectResult (as success result)
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP(INT) response status code</param>
        /// <returns>Return <see cref="ObjectResult"/> from common <see cref="Result{T}"/> as success (HTTP status = 200 or 204)</returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static ObjectResult AsSuccessObjectResult<T>(this Result<T> result, int statusCode)
            => statusCode.IsSuccessHttpStatus() || result.IsNull()
                ? new ObjectResult(null) { StatusCode = statusCode }
                : new ObjectResult(result.Response) { StatusCode = statusCode };

        /// <summary>
        ///     Result to ObjectResult (as success result)
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP response status code</param>
        /// <returns>Return <see cref="ObjectResult"/> from common <see cref="Result{T}"/> as success with HTTP status code</returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static ObjectResult AsSuccessObjectResult<T>(this Result<T> result, HttpStatusCode statusCode)
            => statusCode.IsOkNoContent() || result.IsNull()
                ? new ObjectResult(null) { StatusCode = statusCode.ToInt() }
                : new ObjectResult(result.Response) { StatusCode = statusCode.ToInt() };

        /// <summary>
        ///     Result to ObjectResult (as success result)
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP(INT) response status code</param>
        /// <returns>Return <see cref="ObjectResult"/> from common <see cref="IResult{T}"/> as success with HTTP status code</returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static ObjectResult AsSuccessObjectResult<T>(this IResult<T> result, int statusCode)
            => statusCode.IsSuccessHttpNoContentStatus() || result.IsNull()
                ? new ObjectResult(null) { StatusCode = statusCode }
                : new ObjectResult(result.Response) { StatusCode = statusCode };

        /// <summary>
        ///     Result to ObjectResult (as success result)
        /// </summary>
        /// <param name="result">Common result</param>
        /// <param name="statusCode">HTTP response status code</param>
        /// <returns>Return <see cref="ObjectResult"/> from common <see cref="IResult{T}"/> as success with HTTP status code</returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        public static ObjectResult AsSuccessObjectResult<T>(this IResult<T> result, HttpStatusCode statusCode)
            => statusCode.IsOkNoContent()
                ? new ObjectResult(null) { StatusCode = statusCode.ToInt() }
                : new ObjectResult(result.Response) { StatusCode = statusCode.ToInt() };
    }
}