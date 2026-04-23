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

namespace RzR.ResultMessage.Web.Helpers.Store
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
                { nameof(HttpStatusCode.Continue), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.2.1" },
                { nameof(HttpStatusCode.SwitchingProtocols), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.2.2" },
                { nameof(HttpStatusCode.Processing), "https://datatracker.ietf.org/doc/html/rfc2518#section-10.1" },
                { nameof(HttpStatusCode.EarlyHints), "https://datatracker.ietf.org/doc/html/rfc8297#section-2" },

                //  2xx - Success
                { nameof(HttpStatusCode.OK), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.3.1" },
                { nameof(HttpStatusCode.Created), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.3.2" },
                { nameof(HttpStatusCode.Accepted), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.3.3" },
                { nameof(HttpStatusCode.NonAuthoritativeInformation), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.3.4" },
                { nameof(HttpStatusCode.NoContent), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.3.5" },
                { nameof(HttpStatusCode.ResetContent), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.3.6" },
                { nameof(HttpStatusCode.PartialContent), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.3.7" },
                { nameof(HttpStatusCode.MultiStatus), "https://datatracker.ietf.org/doc/html/rfc4918#section-11.1" },
                { nameof(HttpStatusCode.AlreadyReported), "https://datatracker.ietf.org/doc/html/rfc5842#section-7.1" },
                { nameof(HttpStatusCode.IMUsed), "https://datatracker.ietf.org/doc/html/rfc3229#section-10.4.1" },

                //  3xx - Redirection
                { nameof(HttpStatusCode.Ambiguous), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.1" },
                { nameof(HttpStatusCode.MultipleChoices), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.1" },
                { nameof(HttpStatusCode.Moved), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.2" },
                { nameof(HttpStatusCode.MovedPermanently), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.2" },
                { nameof(HttpStatusCode.Found), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.3" },
                { nameof(HttpStatusCode.Redirect), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.3" },
                { nameof(HttpStatusCode.RedirectMethod), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.4" },
                { nameof(HttpStatusCode.SeeOther), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.4" },
                { nameof(HttpStatusCode.NotModified), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.5" },
                { nameof(HttpStatusCode.UseProxy), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.6" },
                { nameof(HttpStatusCode.Unused), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.7" },
                { nameof(HttpStatusCode.RedirectKeepVerb), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.8" },
                { nameof(HttpStatusCode.TemporaryRedirect), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.8" },
                { nameof(HttpStatusCode.PermanentRedirect), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.4.9" },

                //  4xx - Client Error
                { nameof(HttpStatusCode.BadRequest), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1" },
                { nameof(HttpStatusCode.Unauthorized), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2" },
                { nameof(HttpStatusCode.PaymentRequired), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.3" },
                { nameof(HttpStatusCode.Forbidden), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4" },
                { nameof(HttpStatusCode.NotFound), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5" },
                { nameof(HttpStatusCode.MethodNotAllowed), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.6" },
                { nameof(HttpStatusCode.NotAcceptable), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.7" },
                { nameof(HttpStatusCode.ProxyAuthenticationRequired), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.8" },
                { nameof(HttpStatusCode.RequestTimeout), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.9" },
                { nameof(HttpStatusCode.Conflict), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10" },
                { nameof(HttpStatusCode.Gone), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.11" },
                { nameof(HttpStatusCode.LengthRequired), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.12" },
                { nameof(HttpStatusCode.PreconditionFailed), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.13" },
                { nameof(HttpStatusCode.RequestEntityTooLarge), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.14" },
                { nameof(HttpStatusCode.RequestUriTooLong), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.15" },
                { nameof(HttpStatusCode.UnsupportedMediaType), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.16" },
                { nameof(HttpStatusCode.RequestedRangeNotSatisfiable), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.17" },
                { nameof(HttpStatusCode.ExpectationFailed), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.18" },
                { nameof(HttpStatusCode.MisdirectedRequest), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.20" },
                { nameof(HttpStatusCode.UnprocessableEntity), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.21" },
                { nameof(HttpStatusCode.Locked), "https://datatracker.ietf.org/doc/html/rfc4918#section-11.3" },
                { nameof(HttpStatusCode.FailedDependency), "https://datatracker.ietf.org/doc/html/rfc4918#section-11.4" },
                { nameof(HttpStatusCode.UpgradeRequired), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.22" },
                { nameof(HttpStatusCode.PreconditionRequired), "https://datatracker.ietf.org/doc/html/rfc6585#section-3" },
                { nameof(HttpStatusCode.TooManyRequests), "https://datatracker.ietf.org/doc/html/rfc6585#section-4" },
                { nameof(HttpStatusCode.RequestHeaderFieldsTooLarge), "https://datatracker.ietf.org/doc/html/rfc6585#section-5" },
                { nameof(HttpStatusCode.UnavailableForLegalReasons), "https://datatracker.ietf.org/doc/html/rfc7725#section-3" },

                //  5xx - Server Error
                { nameof(HttpStatusCode.InternalServerError), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1" },
                { nameof(HttpStatusCode.NotImplemented), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.2" },
                { nameof(HttpStatusCode.BadGateway), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.3" },
                { nameof(HttpStatusCode.ServiceUnavailable), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.4" },
                { nameof(HttpStatusCode.GatewayTimeout), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.5" },
                { nameof(HttpStatusCode.HttpVersionNotSupported), "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.6" },
                { nameof(HttpStatusCode.VariantAlsoNegotiates), "https://datatracker.ietf.org/doc/html/rfc2295#section-8.1" },
                { nameof(HttpStatusCode.InsufficientStorage), "https://datatracker.ietf.org/doc/html/rfc4918#section-11.5" },
                { nameof(HttpStatusCode.LoopDetected), "https://datatracker.ietf.org/doc/html/rfc5842#section-7.2" },
                { nameof(HttpStatusCode.NotExtended), "https://datatracker.ietf.org/doc/html/rfc2774#section-7" },
                { nameof(HttpStatusCode.NetworkAuthenticationRequired), "https://datatracker.ietf.org/doc/html/rfc6585#section-6" }
            };
    }
}