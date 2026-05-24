> **Note** This repository multi-targets `netstandard2.1`, `net5.0`, `net6.0`, `net7.0`, `net8.0` and `net9.0`.

[![NuGet Version](https://img.shields.io/nuget/v/RzR.ResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RzR.ResultMessage.Web/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/RzR.ResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RzR.ResultMessage.Web)

<details>

  <summary>Old version</summary>
  
[![NuGet Version](https://img.shields.io/nuget/v/AggregatedGenericResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/AggregatedGenericResultMessage.Web/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/AggregatedGenericResultMessage.Web.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/AggregatedGenericResultMessage.Web)

</details>

<br />

The important thing about this repository is to offer the possibility to manage and organize your result/response from infrastructure to user/controller, as the principal repository around which are built all extensions is [`RzR.ResultMessage`](https://www.nuget.org/packages/RzR.ResultMessage).

**In case you wish to use it in your project, u can install the package from <a href="https://www.nuget.org/packages/RzR.ResultMessage.Web" target="_blank">nuget.org</a>** or specify what version you want:

> `Install-Package RzR.ResultMessage.Web -Version x.x.x.x`

## Highlights (v2.x)
* Multi-target: `netstandard2.1` / `net5.0` / `net6.0` / `net7.0` / `net8.0` / `net9.0` from a single package.
* Pluggable `IResultStatusCodeMapper` — centralize status-code resolution, swap globally via `AddWebResultMessageMapper(...)`.
* Pluggable `IProblemDetailsResultFactory` — control ProblemDetails `type` / `title` / `detail` / `instance` / extensions in one place, swap via `AddProblemDetailsResultFactory(...)`.
* MVC exception filter (`AddWebResultExceptionFilter()`) + generalized middleware (`UseResultExceptionMiddleware()`) — auto-translate unhandled exceptions (including `WebResultException`) to ProblemDetails, no per-action `try/catch`.
* Minimal-API adapters (net6.0+): `IResult.ToHttpResult(...)` and `ResultMessageHttpResults.From(...)` — identical wire format as MVC.
* Automatic correlation: `traceId` emitted from `HttpContext.TraceIdentifier` unless explicitly overridden by the caller.

## Content
1. [USING](docs/usage.md)
2. [MIGRATION v1.x → v2.x](docs/migration-v2.md)
3. [CHANGELOG](docs/CHANGELOG.md)
4. [BRANCH-GUIDE](docs/branch-guide.md)