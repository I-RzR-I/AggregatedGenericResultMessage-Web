> **Note** This repository multi-targets `netstandard2.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0` and `net9.0`.

[![NuGet Version](https://img.shields.io/nuget/v/RzR.ResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RzR.ResultMessage.Web/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/RzR.ResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RzR.ResultMessage.Web)

<details>

  <summary>Old version</summary>
  
[![NuGet Version](https://img.shields.io/nuget/v/AggregatedGenericResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/AggregatedGenericResultMessage.Web/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/AggregatedGenericResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/AggregatedGenericResultMessage.Web)

</details>

<br />


Turn `Result` and `Result<T>` values into ASP.NET Core HTTP responses.

Results produced deep in your infrastructure reach the client without per-action
mapping code, and failures come back as RFC 7807 ProblemDetails with a
machine-readable error code.

Built on [RzR.ResultMessage](https://www.nuget.org/packages/RzR.ResultMessage).

## Install

```
Install-Package RzR.ResultMessage.Web
```

Targets `netstandard2.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0`, `net9.0`.

## Quick start

```csharp
[HttpGet("orders/{id}")]
public IActionResult Get(int id)
    => _service.GetOrder(id).ToProblemResponse();
```

Success returns the payload (or `204` when there is none). Failure returns:

```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1",
  "title": "Order not found",
  "status": 404,
  "detail": "Message: Order not found",
  "code": "E404-OrderNotFound",
  "extensions": {
    "ResultMessages": [ { "key": "E404-OrderNotFound", "message": { "info": "Order not found" } } ],
    "traceId": "0HN7A2QJ8K3PL:00000001"
  }
}
```

Author the code where you create the result:

```csharp
result.WithError("Order not found", "E404-OrderNotFound");      // code is the 2nd argument
Result<Order>.Failure("E404-OrderNotFound", "Order not found"); // code is the 1st
```

## The error code

`code` is a short, stable, machine-readable identifier for the specific failure —
distinct from the HTTP status, and from the human-readable `title` / `detail`.

It is taken from the `Key` of **the same message that supplies `title` and
`detail`**, so a response can never pair a code with an unrelated message. It is
validated against `[A-Za-z0-9._-]` with a 64-character limit, and **omitted**
rather than truncated or rewritten when it does not match — the unmodified value
always remains in `extensions.ResultMessages`.

> `code` is visible to any caller who can see the error response. Do not encode
> internal identifiers or PII in message keys, and use one shared code for all
> authentication failures rather than distinguishing "user not found" from
> "wrong password".

## What you can return

| Method | Body on failure |
|---|---|
| `ToProblemResponse()` | RFC 7807 ProblemDetails — **works in MVC *and* Minimal APIs** |
| `AsProblemDetails()` | RFC 7807 ProblemDetails (MVC) |
| `ToHttpResult()` / `ResultMessageHttpResults.From()` | RFC 7807 ProblemDetails (Minimal API, net6.0+) |
| `AsActionResult()` / `AsIActionResult()` | the `Messages` collection |
| `AsEnvelopeActionResult()` | the whole `Result` envelope |
| `AsSuccessObjectResult()` | success payloads only |

`ToProblemResponse()` returns a single value that is simultaneously a valid MVC
`IActionResult` and a Minimal-API `IResult`, so both hosts emit a byte-identical
wire format.

## Exceptions become ProblemDetails

No per-action `try/catch`. Pick the MVC filter or the host-wide middleware:

```csharp
services.AddWebResultExceptionFilter();          // MVC
// or
services.AddResultExceptionMiddleware(o =>
{
    o.DefaultUnhandledStatusCode = HttpStatusCode.InternalServerError;
    o.IncludeExceptionMessageInDetail = false;   // default — keep false in production
});
app.UseResultExceptionMiddleware();
```

Throw `WebResultException` to surface a `Result` from anywhere in the call stack.
Arbitrary exceptions are translated too, with exception detail **off by default**.

## Two extension points

Both are registered once and apply everywhere:

```csharp
services.AddWebResultMessageMapper<MyStatusCodeMapper>();     // IResultStatusCodeMapper
services.AddProblemDetailsResultFactory<MyProblemFactory>();  // IProblemDetailsResultFactory
```

`IResultStatusCodeMapper` decides which HTTP status a `Result` maps to.
`IProblemDetailsResultFactory` controls `type`, `title`, `detail`, `instance`,
`code` and extensions in one place — override the `Resolve*` hooks to brand every
response without touching a single controller.

## Correlation

`traceId` is populated automatically from `HttpContext.TraceIdentifier` whenever an
ambient context is available, unless the caller supplies one explicitly.

## Content
1. [USING](docs/usage.md)
2. [MIGRATION v1.x → v2.x](docs/migration-v2.md)
3. [CHANGELOG](docs/CHANGELOG.md)
4. [BRANCH-GUIDE](docs/branch-guide.md)