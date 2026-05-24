// ***********************************************************************
//  Assembly          : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author            : RzR
//  Created           : 24-05-2026 21:05
// 
//  Last Modified By : RzR
//  Last Modified On : 24-05-2026 22:02
//  ***********************************************************************
//  <copyright file="ResultMessageProblemDetailsConverter.cs" company="RzR SOFT & TECH">
//      Copyright (c) RzR. All rights reserved.
//  </copyright>
//  <contact>
//      https://iamrzr.dev/contact
//  </contact>
//  <summary></summary>
//  ***********************************************************************

#if NET5_0_OR_GREATER

#region U S I N G

using RzR.ResultMessage.Web.Extensions.Internal.DataType;
using RzR.ResultMessage.Web.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace RzR.ResultMessage.Web.Helpers
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Custom STJ converter for <see cref="ResultMessageProblemDetails" />.
    ///     Overrides <c>ProblemDetailsJsonConverter</c> (which the base class inherits via
    ///     <c>[JsonConverter]</c>) so that all entries in <c>Extensions</c> are written under a
    ///     single nested <c>"extensions"</c> JSON property rather than as top-level properties.
    ///     This matches the structure expected by consumers and by the test suite.
    /// </summary>
    /// <seealso cref="T:System.Text.Json.Serialization.JsonConverter{ResultMessageProblemDetails}" />
    /// =================================================================================================
    internal sealed class ResultMessageProblemDetailsConverter : JsonConverter<ResultMessageProblemDetails>
    {
        /// <inheritdoc />
        /// <remarks>
        ///     <see cref="ResultMessageProblemDetails" /> is a server-side response type.
        ///     Deserialization is not supported.
        /// </remarks>
        public override ResultMessageProblemDetails Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException(
                $"{nameof(ResultMessageProblemDetails)} is a response-only type; deserialization is not supported.");

        /// <inheritdoc />
        public override void Write(
            Utf8JsonWriter writer, ResultMessageProblemDetails value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.Type.IsNotNull())
                writer.WriteString("type", value.Type);

            if (value.Title.IsNotNull())
                writer.WriteString("title", value.Title);

            if (value.Status.HasValue)
                writer.WriteNumber("status", value.Status.Value);

            if (value.Detail.IsNotNull())
                writer.WriteString("detail", value.Detail);

            if (value.Instance.IsNotNull())
                writer.WriteString("instance", value.Instance);

            if (value.Extensions is { Count: > 0 })
            {
                writer.WritePropertyName("extensions");
                JsonSerializer.Serialize(writer, value.Extensions, options);
            }

            writer.WriteEndObject();
        }
    }
}

#endif