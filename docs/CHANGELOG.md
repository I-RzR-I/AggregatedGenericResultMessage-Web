### **v3.1.0.7713** [[RzR](mailto:108324929+I-RzR-I@users.noreply.github.com)] 02-07-2026
* [a092d01] (RzR) -> Auto commit uncommited files
* [6138651] (RzR) -> Unify MVC and Minimal-API problem-details responses.

### **v3.0.0.7978** [[RzR](mailto:108324929+I-RzR-I@users.noreply.github.com)] 24-05-2026
* [80d1431] (RzR) -> **[BREAKING]** Rename `additionInformation` → `additionalInformation` across all public APIs: `ResultProblemDetailsContext.AdditionalInformation`, `WebResultException.AdditionalInformation`, `ResultToProblemDetails` overloads, `ResultToHttpResult` overloads, `ResultMessageHttpResults.From` overloads, `ResultProblemDetailsHelper` overloads, `DefaultProblemDetailsResultFactory`, `WebResultExceptionFilter`, `WebResultExceptionMiddleware` (14 files total).
* [80d1431] (RzR) -> Fix `ResultMessageProblemDetails.Extensions` shadow property: guard the property declaration with `#if NETSTANDARD2_1` since `Microsoft.AspNetCore.Mvc 2.1.3` (netstandard2.1 target) does not expose `ProblemDetails.Extensions`; net5.0+ already has it on the base class.
* [80d1431] (RzR) -> Add `net5.0` target framework: add `net5.0` to `<TargetFrameworks>` with `<FrameworkReference Include="Microsoft.AspNetCore.App" />` alongside the existing net6.0–net9.0 targets.
* [80d1431] (RzR) -> Add `ResultMessageProblemDetailsConverter` (net5.0+): custom `System.Text.Json` `JsonConverter<ResultMessageProblemDetails>` that serializes `Extensions` entries under a nested `"extensions"` JSON property instead of the top-level promotion performed by the inherited `ProblemDetailsJsonConverter`. Applied via `[JsonConverter]` attribute on `ResultMessageProblemDetails` for net5.0+.
* [80d1431] (RzR) -> `WebResultExceptionFilter`: implement `IAsyncExceptionFilter` alongside `IExceptionFilter`. Extracted handling logic into `private static HandleException`; ASP.NET Core always dispatches the async path when both interfaces are present, avoiding unnecessary thread-pool blocking in high-throughput scenarios.
* [80d1431] (RzR) -> Fix docs: rename `additionInformation` → `additionalInformation` in `docs/usage.md` (4 occurrences) and `docs/migration-v2.md` (2 occurrences).

### **v2.0.0.7588** [[RzR](mailto:108324929+I-RzR-I@users.noreply.github.com)] 23-04-2026
* [525a26a] (RzR) -> Auto commit uncommited files
* [2fa8d49] (RzR) -> Adapt the documentation.
* [39c6f57] (RzR) -> Add auto populate for Correlation / TraceId: ProblemDetails.Extensions["traceId"]
* [0668f8e] (RzR) -> Add `.net6.0+` support and `Minimal API` support/adapter.
* [007b011] (RzR) -> Add `IExceptionFilter` and middleware for auto exception handle.
* [fead9c0] (RzR) -> Add a `ProblemDetailsResultFactory` registered in DI.
* [e56e34e] (RzR) -> Add possibility to allow a configurable `IResult` -> `HttpStatusCode` mapper (DI registerable, default = 400).
* [f5f9f76] (RzR) -> Return all result messages on failure (not just the first), also add tests.
* [5e7081f] (RzR) -> Refactor code from `ResultToActionResultByCode`.
* [0b36915] (RzR) -> Switched all URLs from `tools.ietf.org/doc/html/...` to `datatracker.ietf.org/doc/html/...`
* [873d72f] (RzR) -> Use `TryGetValue` in `BuildBaseResultMessageProblemDetails`.
* [60a8c29] (RzR) -> Mark `Extensions` shadow property with `new` key word.
* [dfd0678] (RzR) -> Remove static `_httpStatusCode`, pass it as a local.
* [4aadbc1] (RzR) -> Change namespace from `AggregatedGenericResultMessage.Web` to `RzR.ResultMessage.Web`. Upgrade package reference version.

### **v1.2.1.8615** [[RzR](mailto:108324929+I-RzR-I@users.noreply.github.com)] 24-02-2026
* [5f1076d] (RzR) -> Auto commit uncommited files
* [5f1076d] (RzR) -> Upgrade reference package version (fixed version)

### **v1.2.0.8001** [[RzR](mailto:108324929+I-RzR-I@users.noreply.github.com)] 09-02-2026
* [6533544] (RzR) -> Auto commit uncommited files
* [9448930] (RzR) -> Add new script for version gen.
* [a1c4336] (RzR) -> Fix name from `AsToProblemDetails`, to `AsProblemDetails`.
* [049b4dd] (RzR) -> Upgrade reference package version.

### **v1.1.0.0** 
* [] (RzR) -> Add new/edit data type extensions.<br />
* [] (RzR) -> Upgrade ref package version.<br />
* [] (RzR) -> Add and adjust existing message store and HTTP code rfc link.<br />
* [] (RzR) -> Add new ProblemDetails result model. Extend existing Microsoft.AspNetCore.Mvc.ProblemDetails.<br />
* [] (RzR) -> Add cast helper from `Result` > `ProblemDetails`. Add API `ObjectResult` extension methods.<br />

### **v1.0.5.5444** 
* [] (RzR) -> Update reference package version, fixing CVE (`CVE-2024-43485`).<br />

### **v1.0.4.7652** 
* [] (RzR) -> Update reference package version. <br />
* [] (RzR) -> Fix some warnings.<br />

### **v1.0.3.6912** 
* [] (RzR) -> Update reference package version.

### **v1.0.2.1322** 
* [] (RzR) -> Update reference package version.
* [] (RzR) -> Adjust code to be more readable.
* [] (RzR) -> Add new internal extensions.

### **v.1.0.1.0546** 
* [] (RzR) -> Update reference package version.
