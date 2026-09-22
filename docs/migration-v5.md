# Migration guide: v4.0 to v5.0

v5.0 is a major release. Two changes break you:

1. `ResultBaseApiController` derives from `ControllerBase` instead of `Controller`, on every target framework.
2. The `netstandard2.1` target drops the `Microsoft.AspNetCore.Mvc` metapackage in favour of five narrow package references. That carries one wire-visible side effect, on that target only. See the note in [section 2](#2-resultbaseapicontroller-now-derives-from-controllerbase).

Nothing else moved. Extension methods, mappers, factories, filters, middleware, Minimal-API helpers, namespaces and the ProblemDetails shape are all source- and wire-compatible with v4.0 (`4.0.0.7735`).

---

## Coming from v3.1?

v4.0 (`4.0.0.7735`) was short-lived, so skipping it is a reasonable path. Both breaking changes above reach you unchanged. You also inherit exactly one extra thing, which shipped in v4.0 and is therefore *not* a v5.0 change:

* **Failure ProblemDetails bodies carry a top-level `code` member.** Additive, automatic, no opt-in. [Section 4](#4-failure-bodies-carry-a-code-member) has the detail; read it only if you are coming from v3.1.

Two more v4.0 facts change what you see in section 3:

* v4.0 moved the `netstandard2.1` metapackage pin from `Microsoft.AspNetCore.Mvc` 2.1.3 to 2.3.13.
* It also raised `Microsoft.Extensions.DependencyInjection.Abstractions` from 3.1.32 to 8.0.2. v5.0 puts it back to 3.1.32. See [section 3.2](#32-what-you-gain).

Sections 1 and 3 call out the v3.1 numbers wherever they differ.

---

## Breaking changes at a glance

| Change | Symptom | Loud or silent | Fix |
|--------|---------|----------------|-----|
| `ControllerBase` base class | `Json(...)` does not resolve in your action | Loud: `CS0103` | `new JsonResult(x)` or `Ok(x)` ([section 2.2](#22-jsonvalue-is-gone)) |
| `ControllerBase` base class | `override OnActionExecuting` / `OnActionExecuted` / `OnActionExecutionAsync` | Loud: `CS0115` | Implement `IActionFilter` / `IAsyncActionFilter` ([section 2.3](#23-onactionexecuting-overrides)) |
| `ControllerBase` base class | `override Dispose(bool)` | Loud: `CS0115` | Implement `IDisposable`, or use scoped DI ([section 2.4](#24-disposebool-overrides)) |
| `ControllerBase` base class | `View()`, `ViewBag`, `ViewData`, `TempData`, `ViewComponent()` | Loud: `CS0103` / `CS0117` | Derive from `Controller` and use the extension methods ([section 2.5](#25-views-viewbag-tempdata)) |
| `ControllerBase` base class | Controller discovery via `typeof(Controller).IsAssignableFrom(t)` stops matching | **Silent** | Test against `ControllerBase` ([section 2.6](#26-controller-discovery-silent)) |
| `ControllerBase` base class | v4- or v3-compiled assembly binary-swapped onto 5.0: action-filter overrides and `Dispose(bool)` never run | **Silent** | Recompile. See the warning below |
| netstandard2.1 dependencies | `CS0246` on `JsonConvert`, `JsonPatchDocument<T>`, `IMemoryCache`, ... in a netstandard2.1 class library | Loud: `CS0246` | Add an explicit `PackageReference` ([section 3.3](#33-what-you-may-lose-netstandard21-class-libraries-only)) |
| netstandard2.1 dependencies | `NU1605` package downgrade on restore | Loud: `NU1605` | Raise your direct pin ([section 3.5](#35-floor-changes-nu1605)) |
| netstandard2.1 dependencies | `JsonResult<T>` / `JsonWholeResult<T>` answer `204` instead of `200` for a `null` payload | **Silent** | netstandard2.1 only (see the note in [section 2](#2-resultbaseapicontroller-now-derives-from-controllerbase)) |
| `code` member *(from v3.1 only; shipped in v4.0)* | Strict clients / snapshot tests reject the extra JSON member | Loud at the client, not at build | Suppress via `ResolveCode` ([section 4](#4-failure-bodies-carry-a-code-member)) |

> **WARNING: you must recompile against 5.0.**
> Do not drop the 5.0 assembly next to a controller assembly that was compiled against 4.0 or 3.x. The CLR loads the derived type without complaint, but MVC no longer sees `IActionFilter` on it, because the interface came from `Controller` and `ControllerBase` does not implement it. Nothing tells you:
>
> * your `OnActionExecuting` / `OnActionExecuted` override is **never invoked**. Auth checks, tenant resolution and audit hooks written there stop running, and every request succeeds as if they had passed.
> * your `Dispose(bool)` override is **never called**.
>
> Recompiling turns every one of these into a hard compile error (`CS0115`), which is the only reliable way to find them. Rebuild every project that derives from `ResultBaseApiController`.

---

## 1. Target frameworks and dependencies

Target frameworks are unchanged: `netstandard2.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0`, `net9.0`.

| Target | v4.0 (as published) | v5.0 |
|--------|---------------------|------|
| `netstandard2.1` | `Microsoft.AspNetCore.Mvc` 2.3.13<br/>`Microsoft.Extensions.DependencyInjection.Abstractions` 8.0.2 | `Microsoft.AspNetCore.Mvc.Core` 2.1.38<br/>`Microsoft.AspNetCore.Http` 2.1.34<br/>`Microsoft.Extensions.DependencyModel` 3.1.25<br/>`Microsoft.Extensions.Options` 3.1.32<br/>`Microsoft.Extensions.DependencyInjection.Abstractions` 3.1.32 |
| `net5.0` to `net9.0` | `<FrameworkReference Include="Microsoft.AspNetCore.App" />` | unchanged |

`RzR.ResultMessage` stays at 3.0.0.6928 on every target, in both releases.

Coming from v3.1, the left column reads `Microsoft.AspNetCore.Mvc` **2.1.3** and `Microsoft.Extensions.DependencyInjection.Abstractions` **3.1.32**. The metapackage was present in v3.1 and in v4.0 alike, only at different pins, and the DI-abstractions floor is the same in v3.1 and v5.0.

If you target `net5.0` or newer, skip section 3 entirely. Section 2 applies to every target.

---

## 2. `ResultBaseApiController` now derives from `ControllerBase`

```diff
  [ApiController]
- public abstract class ResultBaseApiController : Controller
+ public abstract class ResultBaseApiController : ControllerBase
```

`Controller` lives in `Microsoft.AspNetCore.Mvc.ViewFeatures` and drags in the whole Razor/view stack. This library never renders a view, and an `[ApiController]` is expected to derive from `ControllerBase` anyway. The switch is also what made the `netstandard2.1` dependency reduction in section 3 possible.

The six helpers the base class actually provides are unchanged in name and signature: `JsonResult<T>`, `JsonResultWithNullCheck<T>`, `JsonResult`, `JsonWholeResult<T>`, `JsonWholeResultWithNullCheck<T>`, `JsonWholeResult`.

> **NOTE: one behaviour change, `netstandard2.1` only, and it is wire-visible.** A `null` payload passed to `JsonResult<T>` or `JsonWholeResult<T>` now yields `204 No Content` rather than `200` with a `null` body. Here is why. On `net5.0`+ the success payload is still wrapped in the shared-framework `JsonResult`. On `netstandard2.1` that type left the dependency graph together with the metapackage (section 3), so those two helpers wrap the payload in an `OkObjectResult` restricted to `application/json` instead. Same payload, same content type, and the host still cannot content-negotiate it to XML. `JsonResultWithNullCheck<T>` and `JsonWholeResultWithNullCheck<T>` already returned `204` for `null`, so they are unaffected on every target.

### 2.1 What is no longer inherited

`Controller` adds these over `ControllerBase`. All of them are gone from your derived controllers:

| Category | Members |
|----------|---------|
| Views | `View()`, `PartialView()`, `ViewComponent()`, `ViewData`, `ViewBag`, `TempData` |
| JSON | `Json(object)`, `Json(object, settings)` |
| Filter hooks | `OnActionExecuting`, `OnActionExecuted`, `OnActionExecutionAsync` |
| Lifetime | `Dispose()`, `Dispose(bool)` |

`Controller` also implements `IActionFilter`, `IAsyncActionFilter` and `IDisposable`. `ControllerBase` implements none of them. That interface loss is what makes the binary-swap case silent.

### 2.2 `Json(value)` is gone

```diff
  public IActionResult GetRaw(int id)
  {
-     return Json(_svc.Get(id));
+     return new JsonResult(_svc.Get(id));  // forces JSON; identical to the v4.0 behaviour
  }
```

Use `Ok(value)` instead if you want the response content-negotiated rather than forced to JSON:

```csharp
return Ok(_svc.Get(id));
```

> **NOTE: netstandard2.1 class libraries.** `new JsonResult(x)` will not compile there unless you add the package back yourself. On that target `JsonResult` lives in `Microsoft.AspNetCore.Mvc.Formatters.Json`, which left the dependency graph in v5.0 (section 3). The portable replacement is the one the library itself uses:
>
> ```csharp
> return new OkObjectResult(value) { ContentTypes = { "application/json" } };
> ```
>
> One observable difference: a `null` value yields `204 No Content`, not `200` with a `null` body.

### 2.3 `OnActionExecuting` overrides

`CS0115: no suitable method found to override`. Implement the filter interface on the controller instead. Drop `override`, make the methods `public`:

```csharp
// v4.0 and earlier
public class OrdersController : ResultBaseApiController
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!_tenant.IsResolved) context.Result = Forbid();
        base.OnActionExecuting(context);
    }
}
```

```csharp
// v5.0
public class OrdersController : ResultBaseApiController, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!_tenant.IsResolved) context.Result = Forbid();
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
```

MVC still invokes these per controller: `DefaultApplicationModelProvider` keys off the interface, not off the base class. There is no `base.OnActionExecuting(...)` left to call, and `Controller`'s implementation was empty anyway.

`OnActionExecutionAsync` maps to `IAsyncActionFilter` the same way. Do not implement both interfaces on one controller; MVC dispatches the async path and the sync one never runs.

If the logic is not really controller-specific, move it into a filter and register it once:

```csharp
services.AddControllers(o => o.Filters.Add<TenantGuardFilter>());
```

### 2.4 `Dispose(bool)` overrides

`CS0115` again. Prefer scoped DI services, which the container disposes for you:

```csharp
// v4.0 and earlier
public class OrdersController : ResultBaseApiController
{
    protected override void Dispose(bool disposing)
    {
        if (disposing) _uow.Dispose();
        base.Dispose(disposing);
    }
}
```

```csharp
// v5.0, preferred: register IUnitOfWork as scoped and let DI dispose it with the request
public class OrdersController : ResultBaseApiController
{
    private readonly IUnitOfWork _uow;

    public OrdersController(IUnitOfWork uow) => _uow = uow;
}
```

```csharp
// v5.0, if the controller really does own the resource
public class OrdersController : ResultBaseApiController, IDisposable
{
    public void Dispose() => _uow.Dispose();
}
```

### 2.5 Views, `ViewBag`, `TempData`

A controller that genuinely needs the view stack is not an API controller. Derive from `Controller` directly and call the extension methods; they were never members of the base class:

```csharp
public class ReportsController : Controller
{
    public IActionResult Index(int id)
    {
        var result = _svc.Get(id);

        return result.IsSuccess
            ? View(result.Response)
            : result.AsProblemDetails();
    }
}
```

`AsActionResult()`, `AsProblemDetails()`, `ToHttpResult()` and `ResultMessageHttpResults.From()` work on any controller, or outside a controller entirely. Only the six `Json*` helpers came from the base class.

### 2.6 Controller discovery (silent)

Custom discovery, test assertions and convention scanners that filter on `Controller` stop matching. No compile error, no runtime error. The set just comes back smaller.

```diff
  var controllers = assembly.GetTypes()
-     .Where(t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract);
+     .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);
```

Grep for `typeof(Controller)`, `is Controller` and `as Controller` across the whole solution, test projects and Swagger/convention setup included.

---

## 3. netstandard2.1 package references replaced

Applies only if you consume this library from a `netstandard2.1` target. `net5.0`+ is unaffected.

### 3.1 The change

```diff
  <ItemGroup Condition="'$(TargetFramework)' == 'netstandard2.1'">
-   <PackageReference Include="Microsoft.AspNetCore.Mvc" Version="2.3.13" />
-   <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.2" />
+   <PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="2.1.38" />
+   <PackageReference Include="Microsoft.AspNetCore.Http" Version="2.1.34" />
+   <PackageReference Include="Microsoft.Extensions.DependencyModel" Version="3.1.25" />
+   <PackageReference Include="Microsoft.Extensions.Options" Version="3.1.32" />
+   <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="3.1.32" />
  </ItemGroup>
```

Coming from v3.1, the two removed lines read `Version="2.1.3"` and `Version="3.1.32"` instead.

### 3.2 What you gain

* The resolved transitive graph on that target drops from roughly 145 packages to 50.
* All known transitive security advisories on that target are cleared.
* `Newtonsoft.Json` leaves the graph entirely. The metapackage reached it through `Microsoft.AspNetCore.Mvc.ViewFeatures`; nothing in the new set does.
* **`Microsoft.Extensions.DependencyInjection.Abstractions` drops from 8.0.2 back to 3.1.32.** That is a floor being *lowered*. Good news, not a downgrade forced on you.

The last point is worth stating plainly, because "the library lowered a dependency version" reads like a regression and is not one.

NuGet resolves a package to the **highest** version any reference asks for, direct or transitive. So:

* If your project already references the `Microsoft.Extensions.*` 8.x line, directly or through another dependency, nothing changes. You stay on 8.x. v5.0 asks for *at least* 3.1.32, and 8.x satisfies that.
* Were you still on the 3.1 line? v4.0 dragged you up to 8.0.2 whether you wanted it or not. v5.0 stops doing that.
* Some people evaluated v4.0 and could not take it, because the 8.x floor conflicted with a pinned graph, a policy or an older host. v5.0 removes that blocker. Upgrade.

The same reasoning covers `Microsoft.AspNetCore.Mvc.Core`, `Microsoft.AspNetCore.Http` and `Microsoft.Extensions.Options`. All three also resolve lower in v5.0 than they did in v4.0. See [section 3.5](#35-floor-changes-nu1605).

### 3.3 What you may lose (netstandard2.1 class libraries only)

The `Microsoft.AspNetCore.Mvc` metapackage was present in v3.1 and in v4.0 alike, only at different pins. Dropping it removes about 95 transitive packages. If a `netstandard2.1` **class library** of yours used any of the types below and relied on this library to supply them, without a `PackageReference` of its own, it now fails to compile with `CS0246`.

| Package no longer transitive | Types you may have been using |
|------------------------------|-------------------------------|
| `Newtonsoft.Json` | `JsonConvert`, `JObject`, `JsonSerializerSettings` |
| `Microsoft.AspNetCore.JsonPatch` | `JsonPatchDocument<T>` |
| `Microsoft.AspNetCore.Mvc.ViewFeatures` | `Controller`, `ViewResult`, `ITempDataDictionary` |
| `Microsoft.AspNetCore.Mvc.Formatters.Json` | `JsonResult` (on the 2.x line), `MvcJsonOptions` |
| `Microsoft.AspNetCore.Mvc.DataAnnotations` | DataAnnotations model-validation plumbing |
| `Microsoft.AspNetCore.Mvc.Cors` | `[EnableCors]` |
| `Microsoft.AspNetCore.Mvc.ApiExplorer` | `ApiDescription`, required by Swashbuckle on the 2.x line |
| `Microsoft.AspNetCore.Mvc.Localization` | `IViewLocalizer`, `IHtmlLocalizer` |
| `Microsoft.AspNetCore.Antiforgery` | `IAntiforgery`, `[ValidateAntiForgeryToken]` |
| `Microsoft.AspNetCore.DataProtection` | `IDataProtectionProvider`, `IDataProtector` |
| `Microsoft.Extensions.Caching.Memory` | `IMemoryCache` |
| `Microsoft.CSharp` | required to compile `dynamic` |

The fix is one line per package you actually use:

```diff
  <ItemGroup>
    <PackageReference Include="RzR.ResultMessage.Web" Version="5.0.0" />
+   <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
+   <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="3.1.32" />
  </ItemGroup>
```

Choose the versions yourself. The table above is only about which packages are no longer supplied for you.

### 3.4 `netcoreapp3.x` applications need no change

An app targeting `netcoreapp3.0` or `netcoreapp3.1` gets every one of those types from its `Microsoft.AspNetCore.App` shared framework, whatever this package's `netstandard2.1` graph contains. Nothing to do. The loss case in 3.3 is confined to `netstandard2.1` class libraries.

### 3.5 Floor changes (`NU1605`)

`NU1605` (package downgrade) fires when one of **your direct** pins sits below what something in the graph demands. Only a floor that *rises* can trigger it.

Relative to v4.0, exactly one floor rises. Everything else falls, and a falling floor cannot break restore.

| Package | v5.0 floor | vs v4.0 | vs v3.1 |
|---------|------------|---------|---------|
| `Microsoft.Extensions.DependencyModel` | 3.1.25 | **rises** (was 2.1.0) | **rises** (was 2.1.0) |
| `Microsoft.AspNetCore.Mvc.Core` | 2.1.38 | falls (was 2.3.12) | **rises** (was 2.1.3) |
| `Microsoft.AspNetCore.Http` | 2.1.34 | falls (was 2.3.11) | **rises** (was 2.1.1) |
| `Microsoft.Extensions.Options` | 3.1.32 | falls (was 8.0.2) | **rises** (was 2.1.1) |
| `Microsoft.Extensions.Primitives` | 3.1.32 | falls (was 8.0.0) | **rises** (was 2.1.1) |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 3.1.32 | falls (was 8.0.2) | unchanged |

The "was" columns are what the `Microsoft.AspNetCore.Mvc` metapackage resolved to at that pin, with nothing else in the project. Other references of yours can push any of them higher, which only makes `NU1605` less likely.

So:

* **From v4.0:** the only pin that can trip `NU1605` is a direct `Microsoft.Extensions.DependencyModel` below 3.1.25. Raise it.
* **From v3.1:** raise any direct pin below the v5.0 floor in the table.

---

## 4. Failure bodies carry a `code` member

> Applies **only if you are coming from v3.1**. This shipped in v4.0 (`4.0.0.7735`) and is not a v5.0 change. Already on v4.0? Skip this section.

Additive on the wire, but observable. Full reference: [usage.md, Error `code`](usage.md#error-code).

What a v3.1 consumer needs to know:

* The member appears **automatically** on any failure ProblemDetails body whose selected message carries a `Key`. No configuration, no opt-in. Affected paths: `AsProblemDetails`, `ToHttpResult`, `ResultMessageHttpResults.From`, the exception filter and the exception middleware. `AsActionResult` failure bodies are not ProblemDetails, so they are unaffected.
* The key must be non-blank, at most 64 characters, and match `[A-Za-z0-9._-]`. Anything else and the member is omitted entirely rather than rewritten. See [Validation](usage.md#validation).
* With no usable key it is omitted, not emitted as `null`. See [When `code` is absent](usage.md#when-code-is-absent).

```json
{
    "type": "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
    "title": "Order not found",
    "status": 404,
    "detail": "Order 42 does not exist.",
    "code": "E404-OrderNotFound",
    "extensions": { "ResultMessages": [ ... ] }
}
```

### 4.1 What breaks

Well-behaved JSON clients ignore an unknown member. These do not:

* schema validation with `"additionalProperties": false`;
* `System.Text.Json` with `UnmappedMemberHandling.Disallow`, or `Newtonsoft.Json` with `MissingMemberHandling.Error`;
* golden-file / snapshot tests that assert on whole error bodies.

Update the schema, the DTO or the snapshot. If you cannot, suppress the member.

### 4.2 Suppressing it

No per-call override, no configuration switch. Override the factory hook and return `null`:

```csharp
public sealed class NoCodeProblemFactory : DefaultProblemDetailsResultFactory
{
    protected override string ResolveCode(ResultProblemDetailsContext context) => null;
}
```

```csharp
services.AddProblemDetailsResultFactory<NoCodeProblemFactory>();
```

See [Overriding](usage.md#overriding) for narrower variants, such as suppressing `code` on 5xx only.

### 4.3 Unhandled exceptions

Also from v4.0, opt-in, off by default:

```csharp
services.AddResultExceptionMiddleware(o => o.DefaultUnhandledErrorCode = "SRV.UNEXPECTED");
```

`WebResultExceptionMiddlewareOptions.DefaultUnhandledErrorCode` defaults to `null`. The library mints no code of its own for unhandled exceptions, so upgrading changes nothing on that path unless you set it. Whatever you set becomes part of your own public contract. See [Unhandled exceptions](usage.md#unhandled-exceptions).

---

## 5. Quick checklist

1. Bump the package to 5.0 and **rebuild every project** that derives from `ResultBaseApiController`. Do not binary-swap.
2. Fix `CS0103` on `Json(...)`: `new JsonResult(x)` or `Ok(x)`.
3. Fix `CS0115` on `OnActionExecuting` / `OnActionExecuted` / `OnActionExecutionAsync` by implementing `IActionFilter` or `IAsyncActionFilter`, or by moving the logic into a registered filter.
4. Fix `CS0115` on `Dispose(bool)`: scoped DI, or implement `IDisposable`.
5. Grep for `typeof(Controller)`, `is Controller`, `as Controller`. Nothing else will tell you about these.
6. *(netstandard2.1 class libraries)* Restore, then add an explicit `PackageReference` for each `CS0246` from the table in [section 3.3](#33-what-you-may-lose-netstandard21-class-libraries-only).
7. *(netstandard2.1)* Raise a direct `Microsoft.Extensions.DependencyModel` pin below 3.1.25. Coming from v3.1, also raise any other direct pin below the v5.0 floor in [section 3.5](#35-floor-changes-nu1605).
8. *(netstandard2.1)* Find every action that returns `JsonResult<T>` or `JsonWholeResult<T>` with a payload that can legitimately be `null`. Those now answer `204`, not `200` with a `null` body. Fix the client, or switch the action to a `...WithNullCheck` helper so the `204` is deliberate.
9. *(from v3.1 only)* Check one failure response for the `code` member. Update strict schemas, DTOs and snapshots, or suppress it with a `ResolveCode` override.
10. *(from v3.1 only, optional)* Set `DefaultUnhandledErrorCode` if you want a code on unhandled-exception responses.
