# Generic result message(Web) - USING

This is an extension lib for `AggregatedGenericResultMessage` that can help you to use into web application.

From the beginning in the current repository, I create an extension for `Controller` to use a more comfortable `Result` to `I/ActionResult` and the available methods are:
```csharp
JsonResult<T>(IResult<T> response)
JsonResultWithNullCheck<T>(IResult<T> response)
JsonResult(IResult response)
JsonWholeResult<T>(IResult<T> response)
JsonWholeResultWithNullCheck<T>(IResult<T> response)
JsonWholeResult(IResult response)
```
<hr/>

Available extensions for repository `I/Result` and `I/Result<T>`:
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
    // Status-code resolution — used by AsActionResult / ToHttpResult when no status is passed.
    .AddWebResultMessageMapper() // DefaultResultStatusCodeMapper
    .AddWebResultMessageMapper<MyStatusCodeMapper>() // or a custom one
    .AddWebResultMessageMapper(new MyStatusCodeMapper()) // or an instance

    // ProblemDetails shaping — title/type/detail/instance/code defaults + extensions.
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
        o.IncludeExceptionMessageInDetail = false;      // default — keep false in production
        o.IncludeExceptionDetailsInExtensions = false;  // default — stack trace under extensions.exception
        o.OnException = (ex, http) => logger.LogError(ex, "Unhandled {Path}", http.Request.Path);
    });
```

### Pipeline

```csharp
app.UseResultExceptionMiddleware();   // before UseRouting()
```

The filter (MVC-only) and the middleware (whole pipeline, incl. middleware-level exceptions) overlap intentionally — use one or both depending on where you want the safety net.

### Custom `IResultStatusCodeMapper`

```csharp
public sealed class MyStatusCodeMapper : IResultStatusCodeMapper
{
    public HttpStatusCode Map(IResult result, bool hasResponseBody)
    {
        if (result.IsSuccess) return hasResponseBody ? HttpStatusCode.OK : HttpStatusCode.NoContent;

        // Promote a caller-supplied error code to a specific status. The code lives on the
        // message Key — the same value the `code` member is sourced from. Mirror the `code`
        // selection rule here (first non-Exception message, else the first) and the status
        // and the body's `code` cannot disagree.
        var selected = result.Messages?.FirstOrDefault(m => m.MessageType != MessageType.Exception)
                       ?? result.Messages?.FirstOrDefault();

        return selected?.Key switch
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

Subclass `DefaultProblemDetailsResultFactory` and override the `Resolve*` hooks — `ResolveType`, `ResolveTitle`, `ResolveDetail`, `ResolveInstance`, `ResolveCode` — or `ApplyExtensions`, or implement `IProblemDetailsResultFactory` from scratch.

Per-call values from `ResultProblemDetailsContext` win over the resolved defaults for `title`, `detail` and `instance`. `code` is the exception: it has no per-call argument, so `ResolveCode` is the only way to change it, and returning `null` from that hook suppresses the member. Whatever an override returns is still validated ([see below](#error-code)) — the check runs at the call site and again after `ApplyExtensions` returns, so neither a `ResolveCode` nor an `ApplyExtensions` override can bypass it. A subclass that replaces `Create()` wholesale does bypass it and owns validation itself.

`traceId` is auto-added from `HttpContext.TraceIdentifier` unless the caller supplied one via `AdditionalInformation`.

---

## Exceptions

* `WebResultException(IResult result, HttpStatusCode? statusCode = null)` — throw from any layer; the filter/middleware convert it to a ProblemDetails response using the configured factory.

```csharp
if (order == null)
    throw new WebResultException(
    Result.Failure("E404-OrderNotFound", "Order not found"), 
    HttpStatusCode.NotFound);
```

`Failure` takes the code **first** (`Failure(code, error)`); `WithError` takes it **second** (`WithError(error, code)`). The code lands on the message `Key` either way and becomes the response's [`code`](#error-code).

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

…into the ProblemDetails `Extensions` dictionary, but only if the caller has not already supplied a `traceId` via `additionalInformation`. Caller-supplied values always win.

---

## Error `code`

Failure bodies carry a top-level `code`: a short, machine-readable identifier for the **specific** failure condition, as opposed to the HTTP status (which identifies the *class* of failure) and to `title` / `detail` (which are human-readable prose).

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
            { "key": "E404-OrderNotFound", "message": { "info": "Order not found" } }
        ],
        "traceId": "0HN3AC2BQ1L4M:00000001"
    }
}
```

Message entries are abbreviated above; each carries the full `IMessageModel` shape (`key`, `message`, `messageType`) and its property casing follows the host's serializer naming policy. `extensions.ResultMessages` stays the canonical, per-message record — `code` is a scalar re-projection of the selected message's `key`, not a replacement for the array.

On `net5.0`+ the member name is the literal `"code"`, written by the library's own `System.Text.Json` converter. On `netstandard2.1` (`Microsoft.AspNetCore.Mvc 2.3.13`, which predates `System.Text.Json`) and on any `net5.0`+ host that calls `AddNewtonsoftJson()`, that converter never runs and the host serializes the body, so the member name follows the host's naming strategy — camelCase by default in ASP.NET Core, hence `"code"` out of the box. Only a non-default contract resolver changes it; omission of a blank or invalid `code` is unaffected and holds on every path.

### Authoring a code

`code` is sourced from the message `Key`. Set it with the **second** argument of `WithError` or the **first** argument of `Failure`:

```csharp
new Result { IsSuccess = false }
    .WithError("Order not found", "E404-OrderNotFound");     // WithError(error, code)

Result<Order>.Failure("E404-OrderNotFound", "Order not found");   // Failure(code, error)
```

### Selection

`code` always describes the **same message as `title` and `detail`**. That is a guarantee of the wire contract, not an implementation detail.

One message is selected — the **first whose `MessageType` is not `MessageType.Exception`**, falling back to the **first message** when every message is an exception message — and `title`, `detail` and `code` are all taken from it. `code` is that message's `Key`.

If the selected message has no `Key`, `code` is **omitted**. No later message is consulted, even when one of them carries a perfectly valid key. `extensions.ResultMessages` remains the full canonical record, and is where a client finds the keys carried by the other messages.

**Case 1 — the selected message is unkeyed, a later one is keyed. `code` is absent:**

```json
{
    "title": "UNKEYED-FIRST-INFO",
    "detail": "Message: UNKEYED-FIRST-INFO",
    "extensions": {
        "ResultMessages": [
            { "key": null,     "message": { "info": "UNKEYED-FIRST-INFO" } },
            { "key": "c-E404", "message": { "info": "KEYED-SECOND-INFO"  } }
        ]
    }
}
```

The `code` property is **not present in the body at all** — it is not emitted as `null`. `c-E404` belongs to the second message and stays readable in `extensions.ResultMessages`.

**Case 2 — the selected message is keyed. `code` is *its* key, never a later one:**

```json
{
    "title": "UNKEYED-FIRST-INFO",
    "detail": "Message: UNKEYED-FIRST-INFO",
    "code": "c-E407",
    "extensions": {
        "ResultMessages": [
            { "key": "c-E407", "message": { "info": "UNKEYED-FIRST-INFO" } },
            { "key": "c-E404", "message": { "info": "KEYED-SECOND-INFO"  } }
        ]
    }
}
```

### Validation

The selected value is **validated, never transformed**. It reaches the wire unchanged when it is:

* non-blank,
* at most **64 characters**,
* composed only of `[A-Za-z0-9._-]`.

Anything else and the member is **omitted entirely**. It is never truncated, stripped or rewritten: `extensions.ResultMessages[*].key` carries the original key, so a rewritten `code` would put two disagreeing copies of the same identifier into one response body.

Selection runs before validation. A malformed key on the **selected** message therefore suppresses `code` — it does not fall through to another message's valid key. Validation narrows the single selected key to "emit" or "omit"; it never re-opens the choice of message.

### When `code` is absent

Clients must treat it as optional: branch on it when present, fall back to `status`. It is omitted when

* the response is a success — no problem body is built at all;
* the message that supplied `title` / `detail` has no `Key` — a later keyed message does **not** fill the gap;
* no message on the result carries a non-blank `Key` at all;
* the selected key fails validation;
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

An override must read the message `Key` and nothing else. Do not fall back to `Info`, `Message`, `Details` or an exception message: the unhandled-exception path builds its result from the caught exception's message, so such a fallback routes raw exception text into a publicly exposed member on every 500 response, bypassing the default-off switch that gates exception detail.

### Unhandled exceptions

The middleware synthesizes its failure result from the caught exception, so that result has no key of its own. `WebResultExceptionMiddlewareOptions.DefaultUnhandledErrorCode` supplies one:

```csharp
services.AddResultExceptionMiddleware(o => o.DefaultUnhandledErrorCode = "SRV.UNEXPECTED");
```

It defaults to `null` on purpose: the library mints no code of its own, because a shipped value would immediately become something consumers branch on. Whatever you set becomes part of **your** public API contract — choose it once and keep it stable.

The value is subject to the same [validation](#validation) as any other code: non-blank, at most 64 characters, only `[A-Za-z0-9._-]`. An invalid value — a space is the usual culprit, e.g. `"Internal Server Error"` — is silently omitted from every unhandled-500 response: no exception, no log entry, no startup failure. Verify the member is present in one unhandled response before you rely on it.

### Security

> **WARNING**
> `code` is a stable, localization-invariant, machine-readable value visible to every caller who can see the error response. Do not encode internal identifiers, schema or table names, or PII in message keys.

Encoding a distinction you do not want enumerable makes that distinction cheaply and reliably enumerable. The classic cases are `AUTH.USER_NOT_FOUND` vs `AUTH.WRONG_PASSWORD`, and `AUTH.ACCOUNT_LOCKED` vs `AUTH.ACCOUNT_NOT_FOUND`: either pair hands an attacker a user-enumeration oracle. Emit a single shared code for every authentication failure.

`title` and `detail` already leak the same distinctions as prose. `code` is the sharper edge: it upgrades a brittle text-scraping heuristic — one that breaks on rewording and on localization — into a reliable programmatic oracle you have promised to keep stable.

Because `.`, `_` and `-` are unbounded within the 64-character limit, `..` and `...` are valid codes. Never use `code` directly as a filesystem or URL path segment (`locales/{code}.json`, `/errors/{code}`). Treat it as an opaque lookup key and resolve it through a map you control.

---

## Wire format summary

| Scenario                           | HTTP status           | Content-Type                  | Body                                                  |
|------------------------------------|-----------------------|-------------------------------|-------------------------------------------------------|
| Success, `IResult`                 | 204 (or mapper value) | —                             | empty                                                 |
| Success, `IResult<T>`              | 200 (or mapper value) | `application/json`            | `T`                                                   |
| Failure (`AsActionResult` /<br/>`AsIActionResult`) | 4xx/5xx (mapper)      | `application/json`            | `result.Messages` array — **not** ProblemDetails, so no `code` and no `traceId` |
| Failure (`AsProblemDetails` /<br/>`ToHttpResult` / `ResultMessageHttpResults.From`) | 4xx/5xx (mapper)      | `application/problem+json`    | `ResultMessageProblemDetails` with `ResultMessages`, optional `code` + optional `traceId` |

