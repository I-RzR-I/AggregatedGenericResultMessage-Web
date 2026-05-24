# Migration guide — v1.x → v2.x

v2 focuses on three things: **multi-target frameworks**, **pluggable strategies** (status-code mapper + ProblemDetails factory), and **Minimal-API parity**. All existing v1 extension methods remain source-compatible; bump the new version can be done without code changes and adopt the new surfaces incrementally.

---

## 1. Target frameworks

| v1.x            | v2.x                                                         |
|-----------------|--------------------------------------------------------------|
| `netstandard2.1` only | `netstandard2.1`, `net6.0`, `net7.0`, `net8.0`, `net9.0` |

The `netstandard2.1`, still depends on `Microsoft.AspNetCore.Mvc 2.1.3` (unchanged). The `net6.0+` TFMs use `<FrameworkReference Include="Microsoft.AspNetCore.App" />` — **do not** add explicit `Microsoft.AspNetCore.*` package references on those target framerowks; remove them from your `.csproj` if present.

---

## 2. `AsToProblemDetails` to `AsProblemDetails`

Already renamed in v1.2.0.8001, but call out for anyone jumping from earlier v1 releases.

```diff
- result.AsToProblemDetails(HttpStatusCode.BadRequest);
+ result.AsProblemDetails(HttpStatusCode.BadRequest);
```

---

## 3. Registering status-code mapping

**v1.x** — hand-crafted in every call:

```csharp
return result.IsSuccess
    ? result.AsActionResult(HttpStatusCode.OK)
    : result.AsProblemDetails(HttpStatusCode.BadRequest);
```

**v2.x** — register once, then the extension methods resolve the status automatically:

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

**v1.x** — override title/type/etc. per call via method arguments.

**v2.x** — register a factory once:

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

**v1.x** — manual `try/catch` (or a bespoke filter) to translate failures into ProblemDetails.

**v2.x** — two opt-in surfaces ship out of the box:

```csharp
// MVC-only filter
services.AddWebResultExceptionFilter();

// Whole-pipeline middleware (recommended; also catches middleware-level exceptions)
services.AddResultExceptionMiddleware(o =>
{
    o.IncludeUnhandledExceptions = true;  // catch non-WebResultException too
    o.DefaultStatusCode = 500;
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
* Pre-populate `additionalInformation["traceId"]` yourself — the library will not overwrite it.

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
| `RzR.ResultMessage.Web.Middlewares`                 | `WebResultExceptionMiddleware`, `WebResultExceptionMiddlewareOptions` |
| `RzR.ResultMessage.Web.Models`                      | `ResultProblemDetailsContext`          |
| `RzR.ResultMessage.Web.Extensions.MinimalApi`       | `ResultToHttpResult`, `ResultMessageHttpResults` *(net6.0+)* |
| `RzR.ResultMessage.Web.WebDependencyInjection`      | `ServiceCollectionExtensions`, `ApplicationBuilderExtensions` |

---

## 9. Package references cleanup (net6.0+)

If your v1 project explicitly referenced individual `Microsoft.AspNetCore.*` packages to work around netstandard2.1, remove those references when targeting net6.0+; they are now covered by the shared framework.

```diff
- <PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="..." />
- <PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="..." />
```

---

## 10. Quick checklist

1. Bump package to v2.
2. Remove stray `Microsoft.AspNetCore.*` package refs on net6+ TFMs.
3. `services.AddWebResultMessageMapper();` — or register a custom mapper.
4. *(Optional)* `services.AddProblemDetailsResultFactory<MyProblemFactory>();` for global ProblemDetails shape.
5. *(Optional)* `services.AddWebResultExceptionFilter();` **and/or** `services.AddResultExceptionMiddleware(...); app.UseResultExceptionMiddleware();`.
6. *(Optional)* Replace Minimal-API hand-rolled `Results.Json(...)` with `.ToHttpResult(http)`.
7. Verify `traceId` appears in ProblemDetails responses (or opt out in your factory).
