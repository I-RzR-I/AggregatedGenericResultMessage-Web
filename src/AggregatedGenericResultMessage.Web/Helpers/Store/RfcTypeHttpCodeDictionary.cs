// ***********************************************************************
//  Assembly         : RzR.Shared.ResultMessage.AggregatedGenericResultMessage.Web
//  Author           : RzR
//  Created On       : 2024-12-25 13:42
// 
//  Last Modified By : RzR
//  Last Modified On : 2024-12-25 16:20
// ***********************************************************************
//  <copyright file="RfcTypeHttpCodeDictionary.cs" company="RzR SOFT & TECH">
//   Copyright © RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Collections.Generic;
using System.Net;

#endregion

namespace AggregatedGenericResultMessage.Web.Helpers.Store
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Dictionary of rfc type HTTP codes.
    /// </summary>
    /// =================================================================================================
    internal static class RfcTypeHttpCodeDictionary
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        ///     (Immutable) information describing the rfc HTTP status code.
        /// </summary>
        /// =================================================================================================
        internal static readonly Dictionary<string, string> RfcHttpStatusCodeInfo =
            new Dictionary<string, string>
            {
                //  1xx - Informational
                { nameof(HttpStatusCode.Continue), "https://tools.ietf.org/doc/html/rfc7231#section-6.2.1" },
                { nameof(HttpStatusCode.SwitchingProtocols), "https://tools.ietf.org/doc/html/rfc7231#section-6.2.2" },
                { nameof(HttpStatusCode.Processing), "https://tools.ietf.org/doc/html/rfc7231#section-6.2" },
                { nameof(HttpStatusCode.EarlyHints), "https://tools.ietf.org/doc/html/rfc7231#section-6.2" },

                //  2xx - OK
                { nameof(HttpStatusCode.OK), "https://tools.ietf.org/doc/html/rfc7231#section-6.3.1" },
                { nameof(HttpStatusCode.Created), "https://tools.ietf.org/doc/html/rfc7231#section-6.3.2" },
                { nameof(HttpStatusCode.Accepted), "https://tools.ietf.org/doc/html/rfc7231#section-6.3.3" },
                { nameof(HttpStatusCode.NonAuthoritativeInformation), "https://tools.ietf.org/doc/html/rfc7231#section-6.3.4" },
                { nameof(HttpStatusCode.NoContent), "https://tools.ietf.org/doc/html/rfc7231#section-6.3.5" },
                { nameof(HttpStatusCode.ResetContent), "https://tools.ietf.org/doc/html/rfc7231#section-6.3.6" },
                { nameof(HttpStatusCode.PartialContent), "https://tools.ietf.org/doc/html/rfc7231#section-6.3" },
                { nameof(HttpStatusCode.MultiStatus), "https://tools.ietf.org/doc/html/rfc7231#section-6.3" },
                { nameof(HttpStatusCode.AlreadyReported), "https://tools.ietf.org/doc/html/rfc7231#section-6.3" },
                { nameof(HttpStatusCode.IMUsed), "https://tools.ietf.org/doc/html/rfc7231#section-6.3" },

                //  3xx - Redirection
                { nameof(HttpStatusCode.Ambiguous), "https://tools.ietf.org/doc/html/rfc7231#section-6.4" },
                { nameof(HttpStatusCode.MultipleChoices), "https://tools.ietf.org/doc/html/rfc7231#section-6.4.1" },
                { nameof(HttpStatusCode.Moved), "https://tools.ietf.org/doc/html/rfc7231#section-6.4" },
                { nameof(HttpStatusCode.MovedPermanently), "https://tools.ietf.org/doc/html/rfc7231#section-6.4.2" },
                { nameof(HttpStatusCode.Found), "https://tools.ietf.org/doc/html/rfc7231#section-6.4.3" },
                { nameof(HttpStatusCode.Redirect), "https://tools.ietf.org/doc/html/rfc7231#section-6.4" },
                { nameof(HttpStatusCode.RedirectMethod), "https://tools.ietf.org/doc/html/rfc7231#section-6.4" },
                { nameof(HttpStatusCode.SeeOther), "https://tools.ietf.org/doc/html/rfc7231#section-6.4.4" },
                { nameof(HttpStatusCode.NotModified), "https://tools.ietf.org/doc/html/rfc7231#section-6.4" },
                { nameof(HttpStatusCode.UseProxy), "https://tools.ietf.org/doc/html/rfc7231#section-6.4.5" },
                { nameof(HttpStatusCode.Unused), "https://tools.ietf.org/doc/html/rfc7231#section-6.4.6" },
                { nameof(HttpStatusCode.RedirectKeepVerb), "https://tools.ietf.org/doc/html/rfc7231#section-6.4" },
                { nameof(HttpStatusCode.TemporaryRedirect), "https://tools.ietf.org/doc/html/rfc7231#section-6.4.7" },
                { nameof(HttpStatusCode.PermanentRedirect), "https://tools.ietf.org/doc/html/rfc7231#section-6.4" },

                //  4xx - Client Error
                { nameof(HttpStatusCode.BadRequest), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.1" },
                { nameof(HttpStatusCode.Unauthorized), "https://tools.ietf.org/doc/html/rfc7235#section-3.1" },
                { nameof(HttpStatusCode.PaymentRequired), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.2" },
                { nameof(HttpStatusCode.Forbidden), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.3" },
                { nameof(HttpStatusCode.NotFound), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.4" },
                { nameof(HttpStatusCode.MethodNotAllowed), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.5" },
                { nameof(HttpStatusCode.NotAcceptable), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.6" },
                { nameof(HttpStatusCode.ProxyAuthenticationRequired), "https://tools.ietf.org/doc/html/rfc7235#section-3.2" },
                { nameof(HttpStatusCode.RequestTimeout), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.7" },
                { nameof(HttpStatusCode.Conflict), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.8" },
                { nameof(HttpStatusCode.Gone), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.9" },
                { nameof(HttpStatusCode.LengthRequired), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.10" },
                { nameof(HttpStatusCode.PreconditionFailed), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.RequestEntityTooLarge), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.11" },
                { nameof(HttpStatusCode.RequestUriTooLong), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.12" },
                { nameof(HttpStatusCode.UnsupportedMediaType), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.13" },
                { nameof(HttpStatusCode.RequestedRangeNotSatisfiable), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.ExpectationFailed), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.14" },
                { nameof(HttpStatusCode.MisdirectedRequest), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.UnprocessableEntity), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.Locked), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.FailedDependency), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.UpgradeRequired), "https://tools.ietf.org/doc/html/rfc7231#section-6.5.15" },
                { nameof(HttpStatusCode.PreconditionRequired), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.TooManyRequests), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.RequestHeaderFieldsTooLarge), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },
                { nameof(HttpStatusCode.UnavailableForLegalReasons), "https://tools.ietf.org/doc/html/rfc7231#section-6.5" },

                //  5xx - Server Error
                { nameof(HttpStatusCode.InternalServerError), "https://tools.ietf.org/doc/html/rfc7231#section-6.6.1" },
                { nameof(HttpStatusCode.NotImplemented), "https://tools.ietf.org/doc/html/rfc7231#section-6.6.2" },
                { nameof(HttpStatusCode.BadGateway), "https://tools.ietf.org/doc/html/rfc7231#section-6.6.3" },
                { nameof(HttpStatusCode.ServiceUnavailable), "https://tools.ietf.org/doc/html/rfc7231#section-6.6.4" },
                { nameof(HttpStatusCode.GatewayTimeout), "https://tools.ietf.org/doc/html/rfc7231#section-6.6.5" },
                { nameof(HttpStatusCode.HttpVersionNotSupported), "https://tools.ietf.org/doc/html/rfc7231#section-6.6.6" },
                { nameof(HttpStatusCode.VariantAlsoNegotiates), "https://tools.ietf.org/doc/html/rfc7231#section-6.6" },
                { nameof(HttpStatusCode.InsufficientStorage), "https://tools.ietf.org/doc/html/rfc7231#section-6.6" },
                { nameof(HttpStatusCode.LoopDetected), "https://tools.ietf.org/doc/html/rfc7231#section-6.6" },
                { nameof(HttpStatusCode.NotExtended), "https://tools.ietf.org/doc/html/rfc7231#section-6.6" },
                { nameof(HttpStatusCode.NetworkAuthenticationRequired), "https://tools.ietf.org/doc/html/rfc7231#section-6.6" }
            };
    }
}