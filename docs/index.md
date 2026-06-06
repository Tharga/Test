---
_layout: landing
---

# Tharga.Test.Toolkit

Small helper library for **unit testing** in .NET. Two tools — one for asserting that every property and field on an object is populated (useful in mapping / copy tests), and one base class for asserting which assemblies a project is allowed to reference. Targets **.NET Standard 2.0** so it drops into any test project on .NET Framework 4.6.1+ or modern .NET.

## Package

| Package | What it does |
|---|---|
| [Tharga.Test.Toolkit](https://www.nuget.org/packages/Tharga.Test.Toolkit) | `IsAssigned` / `AssignmentIssues` / `TestAssignments` extensions on `object` and `Type`, plus the `DependencyTestBase` class for assembly-dependency tests. |

## Quick start

```
dotnet add package Tharga.Test.Toolkit
```

```csharp
using Tharga.Test.Toolkit;

// Assert that every property and field on `result` was populated.
Assert.True(result.IsAssigned());

// Or inspect exactly which members are missing.
foreach (var issue in result.AssignmentIssues())
{
    Console.WriteLine($"{issue.ObjectName}: {issue.Message}");
}
```

See [Getting started](articles/getting-started.md) for the full setup walkthrough.

## What's in the box

- **Assignment checks** — `IsAssigned()` / `AssignmentIssues()` recursively walk an object graph and report any property or field still holding its default value. Pair with mapping / copy code to catch fields you forgot to copy. See [Assignment checks](articles/assignment-checks.md).
- **Method-roundtrip checks** — `TestAssignments()` invokes each public method with auto-generated specimens and yields `(method, response)` tuples — useful as a smoke test for converters / mappers without writing per-method test code. See [Assignment checks](articles/assignment-checks.md#testassignments).
- **Dependency tests** — `DependencyTestBase` exposes the referenced assemblies of a target assembly, filtered to a configurable ignore list. Subclass it to assert "this project must not depend on X". See [Dependency tests](articles/dependency-tests.md).

## Repo

[github.com/Tharga/Test](https://github.com/Tharga/Test) — source, issues, releases.
