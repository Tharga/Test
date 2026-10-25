# Assignment checks

The `AssignmentExtension` class adds three helpers to `object` and `Type`. They share a single purpose: catch fields that you forgot to populate in mapping, copy, or builder code, *before* the value reaches downstream assertions that wouldn't notice.

## `IsAssigned()`

`object.IsAssigned(string[] ignoreProperties = null)` recursively walks the object graph and returns `true` only if every property and field holds a non-default value.

```csharp
var customer = new Customer { Id = 7, Name = "Ada", Country = "SE" };
Assert.True(customer.IsAssigned());
```

Default values that count as "not assigned":

| Type | Not assigned when |
|---|---|
| `string` | `null` and `""` (string default) |
| numbers: `int`, `long`, `short`, `byte`, `float`, `double`, `decimal`, `nint`, `Half`, `Int128`, `BigInteger`, … | `0` |
| `char` | `'\0'` |
| `bool` | `false` |
| `Guid` | `Guid.Empty` |
| `DateTime` / `DateTimeOffset` / `DateOnly` | `MinValue` |
| `TimeSpan` / `TimeOnly` | `TimeSpan.Zero` / midnight (`TimeOnly.MinValue`) |
| `enum` | the zero-valued member |
| `Nullable<T>`, such as `long?` or `Guid?` | `null`, or the default of `T` |
| `Uri` | `null` |
| other reference types | `null`; a non-null one has its members checked |
| other structs | never as a whole; each member is checked |

> **Watch out for `bool`.** A `false` value is treated as not assigned. If `false` is the genuine business value, add the member to `ignoreProperties` (see below). The same goes for any scalar whose default is a real value, such as a count of `0` or a `TimeOnly` of midnight.

### Which value types are compared as a whole

A value type is compared with its default when it is a *scalar*:

- an `enum`;
- a non-generic struct declared in the `System` namespace. That covers every primitive (`int`, `long`, `bool`, `char`, `double`, `nint`, …) as well as `decimal`, `Guid`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `DateOnly`, `TimeOnly`, `Half`, `Int128` and `UInt128`;
- `BigInteger` or `Complex`.

Any other struct is checked member by member, and each unassigned member is reported with its own path. That covers your own structs and generic structs such as `ValueTuple<…>` and `KeyValuePair<,>`.

A nullable value such as `long?` is reported when it is `null`, and also when it holds the default of the underlying type.

> **Changed in 1.15.** Every scalar is now compared with its default. Before 1.15 only `int`, `decimal`, `double`, `bool`, `DateTime`, `TimeSpan` and enums were. Any other value type was checked member by member instead, so a zero `long`, `float`, `short`, `byte` or `char`, and an empty `Guid` on runtimes before .NET 9, passed as assigned. These are now reported. If an assertion starts failing after the upgrade, the value it names really is unset: assign it, or add it to `ignoreProperties` if the default is the value you want.
>
> Checking those types member by member also misfired the other way. A `DateOnly`, `TimeOnly` or `BigInteger` was reported whatever its value, and so was any `Guid` on .NET 9 and later. A `DateTimeOffset` member overflowed the stack. They are now compared as a whole, so a value that is set passes.

### Collections

`IEnumerable` values (other than `string`) are walked element by element — every item must also be fully assigned. A `null` collection is reported like any other unassigned reference.

The collection object itself is not inspected, only its elements:

- Members declared on the collection type are not checked. A `Title` property on a class deriving from `List<T>`, or on your own `IEnumerable<T>` implementation, is ignored even when it is `null`.
- The collection's own properties, such as `Count`, `Capacity` or `IsReadOnly`, are not checked, so an empty collection counts as assigned.

This is deliberate. Checking those members would report every empty list (`Count` is `0`) and every array (`IsReadOnly` is `false`) as unassigned, and would fail on indexers such as `List<T>[int]`. If a collection type carries data of its own that must be set, assert it separately:

```csharp
Assert.True(order.IsAssigned());
Assert.NotNull(order.Lines.Title);
```

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

When the failure is inside a collection element, `Index` holds the element's position. `ObjectName` names the element by its type, not its position (`Order.Lines.OrderLine` for any element of `Order.Lines`), so read `Index` to tell elements apart.

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
