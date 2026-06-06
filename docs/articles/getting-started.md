# Getting started

## Install

```
dotnet add package Tharga.Test.Toolkit
```

Tharga.Test.Toolkit targets **.NET Standard 2.0**, so it works in any test project on .NET Framework 4.6.1+, .NET Core 2.0+, or modern .NET (including .NET 8 / 9 / 10).

## Namespace

Everything lives under `Tharga.Test.Toolkit`:

```csharp
using Tharga.Test.Toolkit;
```

## Assignment checks — minimal example

`IsAssigned()` walks the object graph and returns `true` only if every property and field holds a non-default value. Use it as a one-line assertion that a mapping / copy populated everything:

```csharp
var customer = new Customer { Id = 7, Name = "Ada", Country = "SE" };
Assert.True(customer.IsAssigned());
```

If you need to know *which* members failed, call `AssignmentIssues()` instead — it returns one `IAssignmentIssue` per missing field:

```csharp
foreach (var issue in customer.AssignmentIssues())
{
    Console.WriteLine($"{issue.ObjectName}: {issue.Message}");
}
```

See [Assignment checks](assignment-checks.md) for the ignore-list, nested-object behaviour, and the `TestAssignments` method-roundtrip helper.

## Dependency tests — minimal example

`DependencyTestBase` exposes the referenced assemblies of a target assembly, filtered to a configurable ignore list. Subclass it to write tests that pin which references are allowed:

```csharp
public class ApiDependencyTests : DependencyTestBase
{
    public ApiDependencyTests()
        : base(typeof(MyApi.Startup).Assembly)
    {
    }

    [Fact]
    public void Api_does_not_reference_EntityFramework()
    {
        Assert.DoesNotContain(GetDependencies(), d => d.Name.StartsWith("EntityFramework"));
    }
}
```

See [Dependency tests](dependency-tests.md) for the custom ignore-list overload and the standard ignored assemblies.

## Next steps

- [Assignment checks](assignment-checks.md) — full walkthrough of `IsAssigned`, `AssignmentIssues`, and `TestAssignments`.
- [Dependency tests](dependency-tests.md) — full walkthrough of `DependencyTestBase`.
