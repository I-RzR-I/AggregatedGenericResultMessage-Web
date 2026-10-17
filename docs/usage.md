# Generic result message(Web) - USING

An extension library for `AggregatedGenericResultMessage`, built for web applications.

The starting point in this repository is `ResultBaseApiController`, an abstract `ControllerBase` you inherit from to turn a `Result` into an `I/ActionResult` with less ceremony. It gives you:
```csharp
JsonResult<T>(IResult<T> response)
JsonResultWithNullCheck<T>(IResult<T> response)
JsonResult(IResult response)
JsonWholeResult<T>(IResult<T> response)
JsonWholeResultWithNullCheck<T>(IResult<T> response)
JsonWholeResult(IResult response)
```
<hr/>

Extensions available on `I/Result` and `I/Result<T>`:
* `AsActionResult/<T>`
* `AsIActionResult/<T>`
* `AsSuccessObjectResult/<T>`

---
```csharp
AsActionResult(this Result result)
AsIActionResult(this Result result)
AsActionResult<T>(this Result<T> result)
AsIActionResult<T>(this Result<T> result)
AsActionResult(this IResult result)
AsIActionResult(this IResult result)
AsActionResult<T>(this IResult<T> result)
AsIActionResult<T>(this IResult<T> result)
```

```csharp
AsActionResult(this Result result, HttpStatusCode statusCode)
AsIActionResult(this Result result, HttpStatusCode statusCode)
AsActionResult<T>(this Result<T> result, HttpStatusCode statusCode)
AsIActionResult<T>(this Result<T> result, HttpStatusCode statusCode)
AsActionResult(this IResult result, HttpStatusCode statusCode)
AsIActionResult(this IResult result, HttpStatusCode statusCode)
AsActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
AsIActionResult<T>(this IResult<T> result, HttpStatusCode statusCode)
```

```csharp
AsSuccessObjectResult(this Result result)
AsSuccessObjectResult(this IResult result)
AsSuccessObjectResult<T>(this Result<T> result, int statusCode)
AsSuccessObjectResult<T>(this Result<T> result, HttpStatusCode statusCode)
AsSuccessObjectResult<T>(this IResult<T> result, int statusCode)
AsSuccessObjectResult<T>(this IResult<T> result, HttpStatusCode statusCode)
```

---

## ProblemDetails (`AsProblemDetails`)

RFC 7807 bodies that reuse the library's `Result` envelope (messages, codes, detail).

```csharp
AsProblemDetails(this IResult result)
AsProblemDetails(this IResult result, HttpStatusCode statusCode)
AsProblemDetails(this IResult result, HttpStatusCode statusCode,
    string message = null, string detailMessage = null,
    string accessedResourceUri = null,
    IDictionary<string, object> additionalInformation = null)

AsProblemDetails<T>(this IResult<T> result)
AsProblemDetails<T>(this IResult<T> result, HttpStatusCode statusCode)
AsProblemDetails<T>(this IResult<T> result, HttpStatusCode statusCode,
    string message = null, string detailMessage = null,
    string accessedResourceUri = null,
    IDictionary<string, object> additionalInformation = null)
```

Failure bodies are `ResultMessageProblemDetails` (a `Microsoft.AspNetCore.Mvc.ProblemDetails` extension) with a top-level machine-readable [`code`](#error-code), an additional `ResultMessages` array and, when an ambient `HttpContext` is available, an auto-populated `traceId`.

---

## Registering defaults (`IServiceCollection`)

Everything is configurable; nothing is mandatory. Pick only what you need.

```csharp
services
    // Status-code resolution: used by AsActionResult / ToHttpResult when no status is passed.
    .AddWebResultMessageMapper() // DefaultResultStatusCodeMapper
    .AddWebResultMessageMapper<MyStatusCodeMapper>() // or a custom one
    .AddWebResultMessageMapper(new MyStatusCodeMapper()) // or an instance

    // ProblemDetails shaping: title/type/detail/instance/code defaults plus extensions.
    .AddProblemDetailsResultFactory<MyProblemFactory>()
    .AddProblemDetailsResultFactory(new MyProblemFactory())

    // MVC exception filter: translates unhandled WebResultException to ProblemDetails.
    .AddWebResultExceptionFilter()

    // Middleware options (used by UseResultExceptionMiddleware).
    // The middleware catches every exception (unless the response has already started,
    // in which case it rethrows). These options only shape the response for the
    // exceptions that are NOT a WebResultException.
    .AddResultExceptionMiddleware(o =>
    {
        o.DefaultUnhandledStatusCode = HttpStatusCode.InternalServerError; // default
        o.DefaultUnhandledTitle = "Unhandled exception";                   // default
        o.DefaultUnhandledErrorCode = "SRV.UNEXPECTED";      // opt-in; null (default) emits no code; an invalid value is dropped silently (see "Unhandled exceptions")
        o.IncludeExceptionMessageInDetail = false;      // default; keep it false in production
        o.IncludeExceptionDetailsInExtensions = false;  // default; stack trace under extensions.exception
        o.OnException = (ex, http) => logger.LogError(ex, "Unhandled {Path}", http.Request.Path);
    });
```

### Pipeline

```csharp
app.UseResultExceptionMiddleware();   // before UseRouting()
```

The filter (MVC only) and the middleware (whole pipeline, including middleware-level exceptions) overlap on purpose. Use one or both, depending on where you want the safety net.

### Custom `IResultStatusCodeMapper`

```csharp
public sealed class MyStatusCodeMapper : IResultStatusCodeMapper
{
    public HttpStatusCode Map(IResult result, bool hasResponseBody)
    {
        if (result.IsSuccess) return hasResponseBody ? HttpStatusCode.OK : HttpStatusCode.NoContent;

        // Promote a caller-supplied error code to a specific status. The code lives on the
        // message Code, the same value the `code` member is sourced from. Mirror the `code`
        // selection rule here (first non-Exception message, else the first) and the status
        // and the body's `code` cannot disagree.
        var selected = result.Messages?.FirstOrDefault(m => m.MessageType != MessageType.Exception)
                       ?? result.Messages?.FirstOrDefault();

        return selected?.Code switch
        {
            "NOT_FOUND"  => HttpStatusCode.NotFound,
            "FORBIDDEN"  => HttpStatusCode.Forbidden,
            "VALIDATION" => HttpStatusCode.UnprocessableEntity,
            _            => HttpStatusCode.BadRequest
        };
    }
}
```

### Custom `IProblemDetailsResultFactory`

Subclass `DefaultProblemDetailsResultFactory` and override the `Resolve*` hooks (`ResolveType`, `ResolveTitle`, `ResolveDetail`, `ResolveInstance`, `ResolveCode`), or `ApplyExtensions`, or implement `IProblemDetailsResultFactory` from scratch.

Per-call values from `ResultProblemDetailsContext` win over the resolved defaults for `title`, `detail` and `instance`. `code` is the exception: it has no per-call argument, so `ResolveCode` is the only way to change it, and returning `null` from that hook suppresses the member. Whatever an override returns is still validated ([see below](#error-code)). The check runs at the call site and again after `ApplyExtensions` returns, so neither a `ResolveCode` nor an `ApplyExtensions` override can bypass it. A subclass that replaces `Create()` wholesale does bypass it and owns validation itself.

`traceId` is auto-added from `HttpContext.TraceIdentifier` unless the caller supplied one via `AdditionalInformation`.

---

## Exceptions

* `WebResultException(IResult result, HttpStatusCode? statusCode = null)`: throw it from any layer. The filter and the middleware convert it to a ProblemDetails response using the configured factory.

```csharp
if (order == null)
    throw new WebResultException(
    Result.Failure("E404-OrderNotFound", "Order not found"), 
    HttpStatusCode.NotFound);
```

`Failure` takes the code **first** (`Failure(code, error)`); `WithError` takes it **second** (`WithError(error, code)`). The code lands on the message `Code` either way and becomes the response's [`code`](#error-code).

---

## Minimal API (net6.0+)

The library exposes the same wire format to Minimal-API endpoints.

```csharp
// Direct extension on IResult / IResult<T>:
app.MapGet("/orders/{id}", (int id, HttpContext http, IOrderService svc)
    => svc.Get(id).ToHttpResult(httpContext: http));

// Or via the facade:
app.MapPost("/orders", (OrderDto dto, HttpContext http, IOrderService svc)
    => ResultMessageHttpResults.From(svc.Create(dto), httpContext: http));
```

Signatures:

```csharp
IResult ToHttpResult (this IResult result, HttpStatusCode? statusCode = null, string message = null, string detailMessage = null, string accessedResourceUri = null, IDictionary<string, object> additionalInformation = null, HttpContext httpContext = null);

IResult ToHttpResult<T>(this IResult<T> result, HttpStatusCode? statusCode = null, string message = null, string detailMessage = null, string accessedResourceUri = null, IDictionary<string, object> additionalInformation = null, HttpContext httpContext = null);

ResultMessageHttpResults.From(IResult   result, /* same options */);
ResultMessageHttpResults.From(IResult<T> result, /* same options */);
```

Passing `httpContext` enables the auto-`traceId` correlation.

---

## Correlation / traceId

When an `HttpContext` is available (filter, middleware, or passed explicitly to the Minimal-API adapters), `DefaultProblemDetailsResultFactory` adds:

```json
{
    "traceId": "<HttpContext.TraceIdentifier>"
}
```

That entry goes into the ProblemDetails `Extensions` dictionary, but only if the caller has not already supplied a `traceId` via `additionalInformation`. Caller-supplied values always win.

---

## Response contracts

The library ships **two** failure wire contracts. Which one you get is decided by the extension you call, not by configuration.

| Family | Entry points | Contract | `code` |
|--------|--------------|----------|--------|
| ProblemDetails | `AsProblemDetails` / `AsProblemDetails<T>` | RFC 7807 | yes |
| Unified | `ToProblemResponse` | RFC 7807 | yes |
| Minimal API | `ToHttpResult` / `ToHttpResult<T>`, `ResultMessageHttpResults.From` | RFC 7807 | yes |
| Exception handling | `UseResultExceptionMiddleware`, `AddWebResultExceptionFilter` | RFC 7807 | yes |
| ActionResult | `AsActionResult` / `AsIActionResult`, with or without an `HttpStatusCode` | pre-RFC 7807 | no |
| Envelope | `AsEnvelopeActionResult` / `AsEnvelopeIActionResult` | pre-RFC 7807 | no |
| Base controller | `ResultBaseApiController.JsonResult` / `JsonWholeResult` and their null-check variants | pre-RFC 7807 | no |

Every RFC 7807 family routes through `IProblemDetailsResultFactory`, which is what populates `code` and `traceId`. The pre-RFC 7807 families never reach that factory: they serialize `result.Messages` or the whole `IResult` envelope as-is, the shape they shipped with before problem details existed and keep for backward compatibility.

Both contracts are deliberate. A failure body without a `code` is not a defect; it means the call site belongs to the older contract. To put a call site on `code`, change the call (`AsActionResult()` to `AsProblemDetails()`, for example). No configuration switch adds `code` to the pre-RFC 7807 families.

---

## Error `code`

Failure bodies **rendered as problem details** carry a top-level `code`: a short, machine-readable identifier for the **specific** failure condition, as opposed to the HTTP status (which identifies the *class* of failure) and to `title` / `detail` (which are human-readable prose).

That covers the `AsProblemDetails`, unified, Minimal-API and exception-handling families. The `AsActionResult` / `AsIActionResult`, envelope and `ResultBaseApiController` families never emit `code`, because they never build a problem body ([see Response contracts](#response-contracts)).

```csharp
Result<Order>.Failure("E404-OrderNotFound", "Order not found")
    .AsProblemDetails(HttpStatusCode.NotFound,
        message: "Order not found",
        detailMessage: "Order 42 does not exist.",
        accessedResourceUri: "/orders/42");
```

```json
{
    "type": "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
    "title": "Order not found",
    "status": 404,
    "detail": "Order 42 does not exist.",
    "instance": "/orders/42",
    "code": "E404-OrderNotFound",
    "extensions": {
        "ResultMessages": [
            { "code": "E404-OrderNotFound", "message": { "info": "Order not found" } }
        ],
        "traceId": "0HN3AC2BQ1L4M:00000001"
    }
}
```

Message entries are abbreviated above. Each one carries the full message shape (`code`, `message`, `messageType`, and others), and its property casing follows the host's serializer naming policy. `extensions.ResultMessages` stays the canonical, per-message record. The top-level `code` is a scalar re-projection of the selected message's `Code`, not a replacement for the array. The array entries have a `code` member of their own, which is a different thing: see [Two members named `code`](#two-members-named-code).

On `net5.0`+ the member name is the literal `"code"`, written by the library's own `System.Text.Json` converter. Two paths skip that converter: `netstandard2.1` (`Microsoft.AspNetCore.Mvc.Core 2.1.38`, a 2.1-era stack that predates `System.Text.Json`), and any `net5.0`+ host that calls `AddNewtonsoftJson()`. On those paths the host serializes the body, so the member name follows the host's naming strategy. ASP.NET Core defaults to camelCase, so you still get `"code"` out of the box, and only a non-default contract resolver changes that. Omission of a blank or invalid `code` is unaffected and holds on every path.

### Two members named `code`

The name `code` now appears at two levels of the same failure body, and the two are not the same thing. Before `RzR.ResultMessage` 4.0.0 the array entries called their member `key`, so the names never collided. They do now.

| | Top-level `code` | `extensions.ResultMessages[*].code` |
| --- | --- | --- |
| Written by | This library | The upstream message model, placed in the body as-is |
| Holds | One value, from the selected message | The code of every message, exactly as your application supplied it |
| Validation | Non-blank, at most 64 characters, `[A-Za-z0-9._-]` | None |
| On a rejected value | Omitted entirely, never truncated, stripped or escaped | Present, unchanged |
| Use it to | Branch on | Diagnose |

> **WARNING**
> The two can disagree. A value the top-level member rejects is absent there and still present in the array. Never read the nested `code` as if it had passed the top-level checks. Treat it as untrusted text: do not render it as markup, and do not use it as a file name or URL path segment.

A message authored with `WithError("Order not found", "bad code!")` shows the split. The space and the `!` both fall outside `[A-Za-z0-9._-]`, so the top level has no `code`, while the array keeps the original:

```json
{
    "title": "Order not found",
    "status": 400,
    "extensions": {
        "ResultMessages": [
            { "code": "bad code!", "message": { "info": "Order not found" } }
        ]
    }
}
```

The pre-RFC 7807 families (`AsActionResult`, the envelope helpers and the `ResultBaseApiController` `Json*` helpers) serialize the messages without building a problem body. Their message entries use `code` too, and there is no top-level `code` beside them. See [Response contracts](#response-contracts).

Migrating a client from the old name? See [migration-v7.md](migration-v7.md).

### Authoring a code

The top-level `code` is sourced from the message `Code`. Set it with the **second** argument of `WithError` or the **first** argument of `Failure`:

```csharp
new Result { IsSuccess = false }
    .WithError("Order not found", "E404-OrderNotFound");     // WithError(error, code)

Result<Order>.Failure("E404-OrderNotFound", "Order not found");   // Failure(code, error)
```

### Selection

`code` always describes the **same message as `title` and `detail`**. That is a guarantee of the wire contract, not an implementation detail.

One message is selected: the **first whose `MessageType` is not `MessageType.Exception`**, falling back to the **first message** when every message is an exception message. `title`, `detail` and `code` all come from that one message, and the top-level `code` is its `Code`.

If the selected message has no `Code`, the top-level `code` is **omitted**. No later message is consulted, even when one of them carries a perfectly valid code. `extensions.ResultMessages` remains the full canonical record, and is where a client finds the codes carried by the other messages.

**Case 1: the selected message has no code, a later one does. The top-level `code` is absent.**

```json
{
    "title": "FIRST-INFO",
    "detail": "Message: FIRST-INFO",
    "extensions": {
        "ResultMessages": [
            { "code": null,     "message": { "info": "FIRST-INFO"  } },
            { "code": "c-E404", "message": { "info": "SECOND-INFO" } }
        ]
    }
}
```

The top-level `code` property is **not present in the body at all**; it is not emitted as `null`. `c-E404` belongs to the second message and stays readable in `extensions.ResultMessages`.

**Case 2: the selected message has a code. The top-level `code` is *its* code, never a later one.**

```json
{
    "title": "FIRST-INFO",
    "detail": "Message: FIRST-INFO",
    "code": "c-E407",
    "extensions": {
        "ResultMessages": [
            { "code": "c-E407", "message": { "info": "FIRST-INFO"  } },
            { "code": "c-E404", "message": { "info": "SECOND-INFO" } }
        ]
    }
}
```

### Validation

The selected value is **validated, never transformed**. It reaches the wire unchanged when it is:

* non-blank,
* at most **64 characters**,
* composed only of `[A-Za-z0-9._-]`.

Anything else and the member is **omitted entirely**. It is never truncated, stripped or rewritten: `extensions.ResultMessages[*].code` carries the original value, so a rewritten top-level `code` would put two different values under the same name into one response body. Omitting it means the top-level `code` is either identical to the nested one or absent.

Selection runs before validation. A malformed code on the **selected** message therefore suppresses the top-level `code`; it does not fall through to another message's valid code. Validation narrows the single selected code to "emit" or "omit". It never re-opens the choice of message.

The top-level `code` and `extensions.ResultMessages` are two views of the same data, not two copies of it: the top-level `code` is the sanitised scalar to branch on, `extensions.ResultMessages[*].code` is the unfiltered record. Both members are called `code`, which makes them easy to confuse (see [Two members named `code`](#two-members-named-code)). They can legitimately disagree. A code that fails validation, or one carried by an exception-typed message that selection skipped, still appears in the array while being withheld from the top level. That is intended: branch on the top-level `code`, read the array to diagnose.

### When `code` is absent

Clients must treat it as optional: branch on it when present, fall back to `status`. It is omitted when

* the call site never rendered a problem body at all. This is by far the largest class: `AsActionResult` / `AsIActionResult` (with or without an `HttpStatusCode`), the `AsEnvelope*` overloads and `ResultBaseApiController`'s `Json*` helpers serialise the raw message collection or the whole result envelope, so there is no problem body to carry a `code` ([see Response contracts](#response-contracts));
* the response is a success, so no problem body is built at all;
* the message that supplied `title` / `detail` has no `Code` (a later message that has one does **not** fill the gap);
* no message on the result carries a non-blank `Code` at all;
* the selected code fails validation;
* `ResolveCode` returned `null` (the supported way to suppress it);
* the body came from a custom `IProblemDetailsResultFactory`, or from a subclass that overrides `Create()` wholesale. Population is best-effort and lives in the built-in factory.

### Overriding

There is no per-call override. Unlike `title` / `detail` / `instance`, `code` cannot be passed to `AsProblemDetails` / `ToHttpResult`; override the hook instead.

```csharp
public sealed class MyProblemFactory : DefaultProblemDetailsResultFactory
{
    protected override string ResolveCode(ResultProblemDetailsContext context)
    {
        // Never expose a code on 5xx; keep the default selected-message behaviour otherwise.
        if ((int)context.StatusCode >= 500) return null;

        return base.ResolveCode(context);
    }
}
```

An override must read the message `Code` and nothing else (`Key` is obsolete upstream as of `RzR.ResultMessage` 4.0.0). Do not fall back to `Info`, `Message`, `Details` or an exception message: the unhandled-exception path builds its result from the caught exception's message, so such a fallback routes raw exception text into a publicly exposed member on every 500 response, bypassing the default-off switch that gates exception detail.

### Unhandled exceptions

The middleware synthesizes its failure result from the caught exception, so that result has no code of its own. `WebResultExceptionMiddlewareOptions.DefaultUnhandledErrorCode` supplies one:

```csharp
services.AddResultExceptionMiddleware(o => o.DefaultUnhandledErrorCode = "SRV.UNEXPECTED");
```

It defaults to `null` on purpose: the library mints no code of its own, because a shipped value would immediately become something consumers branch on. Whatever you set becomes part of **your** public API contract. Choose it once and keep it stable.

The value is subject to the same [validation](#validation) as any other code: non-blank, at most 64 characters, only `[A-Za-z0-9._-]`. An invalid value (a space is the usual culprit, as in `"Internal Server Error"`) is silently omitted from every unhandled-500 response: no exception, no log entry, no startup failure. Check that the member is present in one unhandled response before you rely on it.

### Security

> **WARNING**
> `code` is a stable, localization-invariant, machine-readable value visible to every caller who can see the error response. Do not encode internal identifiers, schema or table names, or PII in message codes. The same applies to the nested `extensions.ResultMessages[*].code`, which is visible to the same callers and is not validated.

Encoding a distinction you do not want enumerable makes that distinction cheaply and reliably enumerable. The classic cases are `AUTH.USER_NOT_FOUND` vs `AUTH.WRONG_PASSWORD`, and `AUTH.ACCOUNT_LOCKED` vs `AUTH.ACCOUNT_NOT_FOUND`: either pair hands an attacker a user-enumeration oracle. Emit a single shared code for every authentication failure.

`title` and `detail` already leak the same distinctions as prose. `code` is the sharper edge: it upgrades a brittle text-scraping heuristic, one that breaks on rewording and on localization, into a reliable programmatic oracle you have promised to keep stable.

Because `.`, `_` and `-` are unbounded within the 64-character limit, `..` and `...` are valid codes. Never use `code` directly as a filesystem or URL path segment (`locales/{code}.json`, `/errors/{code}`). Treat it as an opaque lookup key and resolve it through a map you control.

---

## Wire format summary

| Scenario                           | HTTP status           | Content-Type                  | Body                                                  |
|------------------------------------|-----------------------|-------------------------------|-------------------------------------------------------|
| Success, `IResult`                 | 204 (or mapper value) | none                          | empty                                                 |
| Success, `IResult<T>`              | 200 (or mapper value) | `application/json`            | `T`                                                   |
| Failure (`AsActionResult` /<br/>`AsIActionResult`) | 4xx/5xx (mapper)      | `application/json`            | `result.Messages` array, **not** ProblemDetails, so no `code` and no `traceId` |
| Failure (`AsProblemDetails` /<br/>`ToHttpResult` / `ResultMessageHttpResults.From`) | 4xx/5xx (mapper)      | `application/problem+json`    | `ResultMessageProblemDetails` with `ResultMessages`, optional `code` + optional `traceId` |

