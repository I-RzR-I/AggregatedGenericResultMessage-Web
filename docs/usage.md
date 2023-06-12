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

