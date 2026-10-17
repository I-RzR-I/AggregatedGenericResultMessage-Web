# Migration guide: v6.0 to v7.0, nested `key` becomes `code`

v7.0 upgrades the upstream `RzR.ResultMessage` package from 3.0.0.6928 to 4.0.0. That release renamed one member of the message model, and the rename shows up in your HTTP responses:

* `extensions.ResultMessages[*].key` is now `extensions.ResultMessages[*].code`.

Nothing else on the wire moved. The top-level problem-details `code`, the `MessageType` values, the target framework set and the remaining message members (`message`, `messageType`, `logTraceId`, `relatedObjects`) are the same as in v6.0.

---

## What changed

On the upstream `MessageModel` type:

| | 3.0.0.6928 (v6.0) | 4.0.0 (v7.0) |
| --- | --- | --- |
| Member | `Key` | `Code` |
| JSON name | `"key"` | `"code"` |
| XML element | `<Key>` | `<Code>` |
| `Key` | The serialized member | Still on the type, but `[Obsolete]` and excluded from `System.Text.Json`, `XmlSerializer` and `Newtonsoft.Json` output |

The serializers see it like this:

| Serializer | Before | After |
| --- | --- | --- |
| `System.Text.Json` | `"key"` | `"code"` |
| `XmlSerializer` | `<Key>` | `<Code>` |
| `Newtonsoft.Json` | `Key`, named by the host's naming strategy (`"key"` by default) | `Code`, named by the same strategy (`"code"` by default). `Key` is no longer written, because Newtonsoft honours `[IgnoreDataMember]` |

The rename follows the message model, so it appears wherever this library puts `result.Messages` on the wire. That includes the pre-RFC 7807 families (`AsActionResult`, the envelope helpers, the `ResultBaseApiController` `Json*` helpers), not only problem details. See [Response contracts](usage.md#response-contracts).

### Before and after

The same failure, `Result<Order>.Failure("E404-OrderNotFound", "Order not found").AsProblemDetails(HttpStatusCode.NotFound, ...)`.

v6.0:

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

v7.0:

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

Only the member inside the array entry changed. The top-level `code` was there before and is written the same way.

> **WARNING: the name `code` now appears at two levels, and they are not the same thing.**
> The top-level `code` is validated, capped at 64 characters, and omitted if it fails. The nested `code` is the raw value your application supplied, unvalidated. A rejected value is absent from the top level and still present nested. Branch on the top level, treat the nested one as untrusted diagnostic text. Details are in [Two members named `code`](usage.md#two-members-named-code).

---

## Who is affected

Anyone who reads a message entry's identifier **by name**:

* Clients that parse `extensions.ResultMessages[*].key`. This includes non-.NET clients (browser, mobile, Python, Go and so on). They get no package-manager signal and no compile error. A lenient reader just sees `undefined` or `null` where the identifier used to be, which is easy to miss.
* Clients of the pre-RFC 7807 families, whose bodies are the message array or the whole result envelope. Those bodies have no top-level `code`, so the message entries are the only place the identifier appears.
* XML consumers that select the `<Key>` element.
* Strict clients. A DTO or schema that rejects unknown members fails on the new `code` member. That one is loud.
* Contract tests, JSON schemas and golden-file or snapshot tests that assert on whole error bodies.
* Your own server-side .NET code that reads `message.Key`, including `ResolveCode` overrides and custom `IResultStatusCodeMapper` implementations. It still compiles but raises the obsolete warning (`CS0618`). Switch to `Code`.

## Who is not affected

* Clients that follow the documented guidance: branch on the top-level `code` of a problem-details body, and fall back to `status` when it is absent. That member is written by this library, and its name, its validation rules (`[A-Za-z0-9._-]`, at most 64 characters) and its omit-when-invalid behaviour are unchanged.
* Clients that never read the nested array.
* Anything that depends only on `title`, `detail`, `status`, `instance` or `traceId`.

---

## Why the library adopted the new name

`extensions.ResultMessages` is the upstream data model. This library places it in the body as-is, and the member names come from upstream's own serialization attributes. Keeping the old name would need serializer shims for `System.Text.Json`, `Newtonsoft.Json` and `XmlSerializer`. Those shims would still miss some of the code paths that put `result.Messages` on the wire, and they would undo a rename the upstream owner made on purpose and has already marked for removal (`Key` carries `[Obsolete("Use Code instead. Key will be removed in a future major version.")]`).

There is no switch in this package that restores `key`.

---

## What to do

### 1. Make your readers accept either name

Change the readers first, before any server moves to v7.0. A reader that accepts both works against v6.0 servers, v7.0 servers and a mixed fleet during a staged rollout. Prefer `code` when both are present.

C# with `System.Text.Json`:

```csharp
using System.Text.Json;

static string ReadMessageCode(JsonElement message)
{
    if (message.TryGetProperty("code", out var code) && code.ValueKind == JsonValueKind.String)
        return code.GetString();

    if (message.TryGetProperty("key", out var key) && key.ValueKind == JsonValueKind.String)
        return key.GetString();

    return null;
}

// usage
using var doc = JsonDocument.Parse(responseBody);
var messages = doc.RootElement.GetProperty("extensions").GetProperty("ResultMessages");

foreach (var message in messages.EnumerateArray())
    Console.WriteLine(ReadMessageCode(message));
```

JavaScript or TypeScript:

```js
const messageCode = (m) => m.code ?? m.key;
```

XML consumers: select `Code`, and fall back to `Key` when it is missing.

### 2. Upgrade the server

Update the package. Then update your own server-side code that reads `message.Key` to use `message.Code`.

### 3. Update contracts and tests

Rename the member in JSON schemas, DTOs, OpenAPI documents that describe the nested array, and snapshot files.

### 4. Remove the fallback

When every server in the fleet is on v7.0, drop the `key` branch from your readers. The upstream `Key` member is scheduled for removal in a future major version, so the fallback has no long-term use.

---

## Quick checklist

1. Search every client for reads of `ResultMessages[*].key` (and `<Key>` in XML). Include non-.NET clients.
2. Make those reads accept `code` or `key`, and ship that before the server upgrade.
3. Upgrade the package to v7.0.
4. Replace `.Key` with `.Code` in your own server-side code, and clear the `CS0618` warnings.
5. Update schemas, DTOs and snapshot tests for the renamed member.
6. Check that nothing branches on the nested `code`. Branch on the top-level `code`.
7. After the whole fleet is on v7.0, remove the `key` fallback.
