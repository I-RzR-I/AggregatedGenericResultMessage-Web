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

#if NETSTANDARD2_1 || NET5_0_OR_GREATER

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
    ///     Overrides the base <c>ProblemDetailsJsonConverter</c> where the platform supplies one, so that
    ///     all entries in <c>Extensions</c> are written under a single nested <c>"extensions"</c> JSON
    ///     property rather than as top-level properties.
    ///     This matches the structure expected by consumers and by the test suite.
    ///     It also emits the top-level <c>"code"</c> member carrying
    ///     <see cref="ResultMessageProblemDetails.Code" />, and omits that member when the code is blank.
    ///     Compiled on every target so the omission holds on netstandard2.1 hosts too, which serialize
    ///     with System.Text.Json and honor no <c>ShouldSerialize</c> convention.
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
        public override void Write(Utf8JsonWriter writer, 
            ResultMessageProblemDetails value, JsonSerializerOptions options)
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

            if (value.Code.IsMissing().IsFalse())
                writer.WriteString("code", value.Code);

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