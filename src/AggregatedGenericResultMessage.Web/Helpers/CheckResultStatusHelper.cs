// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2023-06-06 00:22
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-06-09 08:39
// ***********************************************************************
//  <copyright file="CheckResultStatusHelper.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Net;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Web.Extensions.Internal.DataType;
using AggregatedGenericResultMessage.Web.Models;
using Microsoft.AspNetCore.Http;

#endregion

namespace AggregatedGenericResultMessage.Web.Helpers
{
    /// <summary>
    ///     Check result status helper
    /// </summary>
    internal static class CheckResultStatusHelper
    {
        /// <summary>
        ///     Check if HTTP status code is success
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns>Return result from HTTP status code</returns>
        /// <remarks></remarks>
        internal static IResult CheckIfIsSuccess(HttpStatusCode statusCode)
        {
            if (statusCode.IsNull())
                throw new ArgumentNullException(nameof(statusCode));

            var isValidStatus = statusCode.ToInt() >= StatusCodes.Status100Continue && statusCode.ToInt() < StatusCodes.Status400BadRequest;

            return isValidStatus.IsTrue()
                ? Result.Success()
                : Result.Failure(MessageStore.HttpStatusCodeNotInSuccessfullyRange);
        }

        /// <summary>
        ///     Check if HTTP status code is success
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns>Return result from HTTP status code</returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        internal static IResult<T> CheckIfIsSuccess<T>(HttpStatusCode statusCode)
        {
            if (statusCode.IsNull())
                throw new ArgumentNullException(nameof(statusCode));

            var isValidStatus = statusCode.ToInt() >= StatusCodes.Status100Continue && statusCode.ToInt() < StatusCodes.Status400BadRequest;

            return isValidStatus.IsTrue()
                ? Result<T>.Success()
                : Result<T>.Failure(MessageStore.HttpStatusCodeNotInSuccessfullyRange);
        }

        /// <summary>
        ///     Check if HTTP status code is error
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns>Return result from HTTP status code</returns>
        /// <remarks></remarks>
        internal static IResult CheckIfIsError(HttpStatusCode statusCode)
        {
            if (statusCode.IsNull())
                throw new ArgumentNullException(nameof(statusCode));

            var isValidStatus = statusCode.ToInt() >= StatusCodes.Status400BadRequest;

            return isValidStatus.IsFalse()
                ? Result.Success()
                : Result.Failure(MessageStore.HttpStatusCodeNotInErrorRange);
        }

        /// <summary>
        ///     Check if HTTP status code is error
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns>Return result from HTTP status code</returns>
        /// <typeparam name="T">Common result type</typeparam>
        /// <remarks></remarks>
        internal static IResult<T> CheckIfIsError<T>(HttpStatusCode statusCode)
        {
            if (statusCode.IsNull())
                throw new ArgumentNullException(nameof(statusCode));

            var isValidStatus = statusCode.ToInt() >= StatusCodes.Status400BadRequest;

            return isValidStatus.IsFalse()
                ? Result<T>.Success()
                : Result<T>.Failure(MessageStore.HttpStatusCodeNotInErrorRange);
        }

        /// <summary>
        ///     Check HTTP status code
        /// </summary>
        /// <param name="statusCode">Current HTTP status code</param>
        /// <returns>Return check result from HTTP code</returns>
        /// <remarks></remarks>
        internal static IResult<CheckHttpStatus> CheckStatusCode(HttpStatusCode statusCode)
        {
            if (statusCode.IsNull())
                throw new ArgumentNullException(nameof(statusCode));

            var httpStatusCode = statusCode.ToInt();
            var isSuccessCode = httpStatusCode >= StatusCodes.Status100Continue && httpStatusCode < StatusCodes.Status400BadRequest;

            return Result<CheckHttpStatus>.Success(
                new CheckHttpStatus
                {
                    IsSuccess = isSuccessCode.IsTrue(),
                    IsError = isSuccessCode.IsFalse()
                });
        }
    }
}