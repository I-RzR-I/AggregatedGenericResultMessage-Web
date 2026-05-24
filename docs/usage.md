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

Failure bodies are `ResultMessageProblemDetails` (a `Microsoft.AspNetCore.Mvc.ProblemDetails` extension) with an additional `ResultMessages` array and, when an ambient `HttpContext` is available, an auto-populated `traceId`.

---

## Registering defaults (`IServiceCollection`)

Everything is configurable; nothing is mandatory. Pick only what you need.

```csharp
services
    // Status-code resolution — used by AsActionResult / ToHttpResult when no status is passed.
    .AddWebResultMessageMapper() // DefaultResultStatusCodeMapper
    .AddWebResultMessageMapper<MyStatusCodeMapper>() // or a custom one
    .AddWebResultMessageMapper(new MyStatusCodeMapper()) // or an instance

    // ProblemDetails shaping — title/type/detail/instance defaults + extensions.
    .AddProblemDetailsResultFactory<MyProblemFactory>()
    .AddProblemDetailsResultFactory(new MyProblemFactory())

    // MVC exception filter: translates unhandled WebResultException to ProblemDetails.
    .AddWebResultExceptionFilter()

    // Middleware options (used by UseResultExceptionMiddleware).
    .AddResultExceptionMiddleware(o =>
    {
        o.IncludeUnhandledExceptions = true;  // also catch non-WebResultException
        o.DefaultStatusCode = 500;
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

        // Promote a caller-supplied error code to a specific status.
        var firstCode = result.Messages?.FirstOrDefault()?.Message?.Code;
        return firstCode switch
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

Subclass `DefaultProblemDetailsResultFactory` and override the `Resolve*` hooks, or implement `IProblemDetailsResultFactory` from scratch. Per-call values from `ResultProblemDetailsContext` always win over resolved defaults; `traceId` is auto-added from `HttpContext.TraceIdentifier` unless the caller supplied one via `AdditionalInformation`.

---

## Exceptions

* `WebResultException(IResult result, HttpStatusCode? statusCode = null)` — throw from any layer; the filter/middleware convert it to a ProblemDetails response using the configured factory.

```csharp
if (order == null)
    throw new WebResultException(
    Result.Failure("Order not found", "E404"), 
    HttpStatusCode.NotFound);
```

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

## Wire format summary

| Scenario                           | HTTP status           | Content-Type                  | Body                                                  |
|------------------------------------|-----------------------|-------------------------------|-------------------------------------------------------|
| Success, `IResult`                 | 204 (or mapper value) | —                             | empty                                                 |
| Success, `IResult<T>`              | 200 (or mapper value) | `application/json`            | `T`                                                   |
| Failure (`AsActionResult` /<br/>`AsProblemDetails` / `ToHttpResult`) | 4xx/5xx (mapper)      | `application/problem+json`    | `ResultMessageProblemDetails` with `ResultMessages` + optional `traceId` |

