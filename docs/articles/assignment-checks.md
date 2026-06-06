# Assignment checks

The `AssignmentExtension` class adds three helpers to `object` and `Type`. They share a single purpose: catch fields that you forgot to populate in mapping, copy, or builder code, *before* the value reaches downstream assertions that wouldn't notice.

## `IsAssigned()`

`object.IsAssigned(string[] ignoreProperties = null)` recursively walks the object graph and returns `true` only if every property and field holds a non-default value.

```csharp
var customer = new Customer { Id = 7, Name = "Ada", Country = "SE" };
Assert.True(customer.IsAssigned());
```

Default values that count as "not assigned":

| Type | Default |
|---|---|
| `string` | `null` and `""` (string default) |
| `int` / `decimal` / `double` | `0` |
| `bool` | `false` |
| `DateTime` | `DateTime.MinValue` |
| `TimeSpan` | `TimeSpan.Zero` |
| `enum` | the zero-valued member |
| `Uri` | `null` |
| reference types | `null` |
| value types | recursively checked member-by-member |

`IEnumerable` properties are walked element by element — every item must also be fully assigned.

> **Watch out for `bool`.** A `false` value is treated as not assigned. If `false` is the genuine business value, add the member to `ignoreProperties` (see below).

### Ignoring specific members

Pass a string array of `"ParentTypeName.MemberName"` entries that should *not* be flagged:

```csharp
var settings = new ServerSettings { Host = "api.example.com", UseTls = false };
Assert.True(settings.IsAssigned(new[] { "ServerSettings.UseTls" }));
```

The same array works for `AssignmentIssues()`.

## `AssignmentIssues()`

`object.AssignmentIssues(string[] ignoreProperties = null)` returns one `IAssignmentIssue` for every member that failed the assignment check. Use it when you want a human-readable list of what's missing instead of a single boolean:

```csharp
var customer = new Customer { Id = 7 };          // Name + Country are still defaults
var issues = customer.AssignmentIssues().ToArray();

Assert.Empty(issues);                            // fails — shows you exactly which fields
```

Each `IAssignmentIssue` carries:

| Member | Meaning |
|---|---|
| `ObjectName` | dotted path to the failing member, e.g. `Customer.Name` |
| `Message` | human-readable description (`"The string value 'Customer.Name' is not assigned."`) |
| `Index` | element index when the issue lives inside an `IEnumerable` element, otherwise `null` |

When the failure is inside a collection element, `ObjectName` substitutes `[]` with the actual index (`Order.Lines[2].Sku`).

## `TestAssignments()`

`TestAssignments()` invokes each public method of a type (or instance) with auto-generated specimen values and yields `(string method, object data)` tuples. It does *not* assert anything — it's a smoke test that every public surface area can be called and returns *something*. Useful as a one-liner against converters / mappers where writing one test per method would be filler.

### Static methods on a type

```csharp
var results = typeof(StringConverter).TestAssignments(MakeSpecimen);

foreach (var (method, data) in results)
{
    Assert.NotNull(data);                        // every static method returned non-null
}
```

`MakeSpecimen` is your factory — given a parameter `Type`, return a representative instance:

```csharp
static object MakeSpecimen(Type t) =>
    t == typeof(string)   ? "x" :
    t == typeof(int)      ? 1   :
    t == typeof(DateTime) ? new DateTime(2026, 1, 1) :
    Activator.CreateInstance(t);
```

### Instance methods on a specific object

```csharp
var converter = new OrderConverter();
var results = converter.TestAssignments(MakeSpecimen);
```

`GetType` and `Equals` are automatically excluded from the instance overload. Pass extra exclusions via `ignoreFunctions`:

```csharp
converter.TestAssignments(MakeSpecimen, ignoreFunctions: new[] { "ToString", "GetHashCode" });
```

`bindingAttr` defaults to `BindingFlags.Static | BindingFlags.Public` for the `Type` overload and `BindingFlags.Instance | BindingFlags.Public` for the instance overload — override it to broaden or narrow what gets called.
