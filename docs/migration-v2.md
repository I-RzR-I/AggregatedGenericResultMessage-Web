# Migration guide: v1.x to v2.x

v2 brings **multi-target frameworks**, **pluggable strategies** (status-code mapper plus ProblemDetails factory) and **Minimal-API parity**. Every v1 extension method stays source-compatible, so you can bump the version without touching code and pick up the new surfaces later, one at a time.

---

## 1. Target frameworks

| v1.x            | v2.x                                                         |
|-----------------|--------------------------------------------------------------|
| `netstandard2.1` only | `netstandard2.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0`, `net9.0` |

The `netstandard2.1` TFM still depends on the `Microsoft.AspNetCore.Mvc` metapackage, pinned at `2.1.3`. v2 does not move that pin. The `net5.0+` TFMs use `<FrameworkReference Include="Microsoft.AspNetCore.App" />` instead, so **do not** add explicit `Microsoft.AspNetCore.*` package references on those target frameworks. Remove them from your `.csproj` if they are already there.

v4 moved that pin to `2.3.13`; v5 drops the metapackage from `netstandard2.1` entirely in favour of narrow package references. If you are moving on to v5, see the [v5 migration guide](migration-v5.md).

---

## 2. `AsToProblemDetails` to `AsProblemDetails`

This rename already landed in v1.2.0.8001. It is listed here for anyone jumping from an earlier v1 release.

```diff
- result.AsToProblemDetails(HttpStatusCode.BadRequest);
+ result.AsProblemDetails(HttpStatusCode.BadRequest);
```

---

## 3. Registering status-code mapping

**v1.x** hand-crafted the status in every call:

```csharp
return result.IsSuccess
    ? result.AsActionResult(HttpStatusCode.OK)
    : result.AsProblemDetails(HttpStatusCode.BadRequest);
```

**v2.x** resolves it from a mapper you register once:

```csharp
// Startup / Program.cs
services.AddWebResultMessageMapper(); // default mapper
// or
services.AddWebResultMessageMapper<MyStatusCodeMapper>(); // custom
```

```csharp
// Controller
return result.AsActionResult(); // status from mapper
return result.AsProblemDetails(); // status from mapper
return result.ToHttpResult(); // Minimal API (net6+)
```

Per-call status codes still win if you pass them explicitly.

> Backwards compatibility: the v1-style overloads (`AsActionResult(result, HttpStatusCode)`, etc.) are unchanged. Adoption is opt-in.

---

## 4. Customizing ProblemDetails

**v1.x** set title, type and the rest through per-call method arguments.

**v2.x** takes a factory you register once:

```csharp
public sealed class MyProblemFactory : DefaultProblemDetailsResultFactory
{
    protected override string ResolveType(ResultProblemDetailsContext ctx)
        => $"https://errors.my-api.example/{ctx.StatusCode:D}";

    protected override void ApplyExtensions(
        ResultMessageProblemDetails problem, ResultProblemDetailsContext ctx)
    {
        base.ApplyExtensions(problem, ctx); // keeps ResultMessages + traceId
        problem.Extensions["service"] = "orders-api";
    }
}
```

```csharp
services.AddProblemDetailsResultFactory<MyProblemFactory>();
```

Per-call arguments to `AsProblemDetails(...)` / `ToHttpResult(...)` still override the factory's defaults.

---

## 5. Automatic exception translation

**v1.x** needed a manual `try/catch`, or a bespoke filter, to translate failures into ProblemDetails.

**v2.x** ships two opt-in surfaces:

```csharp
// MVC-only filter
services.AddWebResultExceptionFilter();

// Whole-pipeline middleware (recommended; also catches middleware-level exceptions)
// The middleware catches every exception; these options only shape the response
// for exceptions that are NOT a WebResultException.
services.AddResultExceptionMiddleware(o =>
{
    o.DefaultUnhandledStatusCode = HttpStatusCode.InternalServerError; // default
    o.DefaultUnhandledTitle = "Unhandled exception";                   // default
    o.IncludeExceptionMessageInDetail = false; // default; keep it false in production
});

app.UseResultExceptionMiddleware(); // before UseRouting()
```

Throw `WebResultException(result, status)` from any layer; the response body matches `AsProblemDetails` 1:1.

---

## 6. Minimal API (net6.0+)

New in v2. Wire format identical to MVC.

```csharp
app.MapGet("/orders/{id}", (int id, HttpContext http, IOrderService svc)
    => svc.Get(id).ToHttpResult(httpContext: http));

app.MapPost("/orders", (OrderDto dto, HttpContext http, IOrderService svc)
    => ResultMessageHttpResults.From(svc.Create(dto), httpContext: http));
```

If you had already emitted `IResult` bodies manually via `Results.Json(result, status)` in v1, replace those sites with `.ToHttpResult(http)` so the ProblemDetails shape stays consistent.

---

## 7. Correlation / traceId

New in v2, zero configuration.

* Filter path, middleware path, and Minimal-API path all forward the ambient `HttpContext` to `DefaultProblemDetailsResultFactory`, which sets `problem.Extensions["traceId"] = HttpContext.TraceIdentifier`.
* If your code passes a `traceId` via `additionalInformation`, that value is preserved (no overwrite).
* To opt out, register a factory that overrides `ApplyExtensions` and skips the `traceId` injection.

If you previously carried correlation on your own, either:
* Remove your bespoke injection and let the library handle it, **or**
* Pre-populate `additionalInformation["traceId"]` yourself. The library will not overwrite it.

---

## 8. Namespace / type changes

No breaking namespace or public-type renames. Newly added public types:

| Namespace                                           | Type                                   |
|-----------------------------------------------------|----------------------------------------|
| `RzR.ResultMessage.Web.Abstractions`                | `IResultStatusCodeMapper`              |
| `RzR.ResultMessage.Web.Abstractions`                | `IProblemDetailsResultFactory`         |
| `RzR.ResultMessage.Web.Mappers`                     | `DefaultResultStatusCodeMapper`, `ResultStatusCodeMapper` (ambient) |
| `RzR.ResultMessage.Web.Factories`                   | `DefaultProblemDetailsResultFactory`, `ProblemDetailsResultFactory` (ambient) |
| `RzR.ResultMessage.Web.Filters`                     | `WebResultExceptionFilter`             |
| `RzR.ResultMessage.Web.Middlewares`                 | `WebResultExceptionMiddleware`         |
| `RzR.ResultMessage.Web.Models`                      | `ResultProblemDetailsContext`, `WebResultExceptionMiddlewareOptions` |
| `RzR.ResultMessage.Web.Extensions.MinimalApi`       | `ResultToHttpResult`, `ResultMessageHttpResults` *(net6.0+)* |
| `RzR.ResultMessage.Web.WebDependencyInjection`      | `ServiceCollectionExtensions`, `ApplicationBuilderExtensions` |

---

## 9. Package references cleanup (net5.0+)

If your v1 project explicitly referenced individual `Microsoft.AspNetCore.*` packages to work around netstandard2.1, remove those references when targeting net5.0+; they are now covered by the shared framework.

```diff
- <PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="..." />
- <PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="..." />
```

---

## 10. Quick checklist

1. Bump package to v2.
2. Remove stray `Microsoft.AspNetCore.*` package refs on net5.0+ TFMs.
3. `services.AddWebResultMessageMapper();` (or register a custom mapper).
4. *(Optional)* `services.AddProblemDetailsResultFactory<MyProblemFactory>();` for global ProblemDetails shape.
5. *(Optional)* `services.AddWebResultExceptionFilter();` **and/or** `services.AddResultExceptionMiddleware(...); app.UseResultExceptionMiddleware();`.
6. *(Optional)* Replace Minimal-API hand-rolled `Results.Json(...)` with `.ToHttpResult(http)`.
7. Verify `traceId` appears in ProblemDetails responses (or opt out in your factory).
