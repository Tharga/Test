# Dependency tests

`DependencyTestBase` is a base class for tests that pin which assemblies a target project is allowed to reference. Subclass it, point it at the assembly under test, and assert against `GetDependencies()` in your test methods.

## Why

`.csproj` references drift quietly. A new transitive `<PackageReference>` can leak EntityFramework, ASP.NET, or another heavyweight assembly into a project that's meant to stay small. A `DependencyTestBase` subclass turns "this project must not reference X" into a unit test that fails on the offending PR, not in production.

## Minimal subclass

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

    [Fact]
    public void Api_only_references_allowed_assemblies()
    {
        var allowed = new[] { "System", "Microsoft.AspNetCore", "Tharga.Mcp" };
        foreach (var dep in GetDependencies())
        {
            Assert.Contains(allowed, prefix => dep.Name.StartsWith(prefix));
        }
    }
}
```

## What `GetDependencies()` returns

`GetDependencies()` calls `Assembly.GetReferencedAssemblies()` on the target assembly and filters out anything whose `Name` matches an entry in the ignore list. The result is a sequence of `AssemblyName` — use `.Name`, `.Version`, or `.FullName` to assert what you care about.

## Constructors

```csharp
protected DependencyTestBase(Assembly assemblyToTest, string[] assembliesToIgnore = null)
protected DependencyTestBase(Assembly assemblyToTest, Assembly[] assembliesToIgnore)
```

| Parameter | Notes |
|---|---|
| `assemblyToTest` | The assembly whose references you want to assert against. Typically `typeof(SomeTypeInThatProject).Assembly`. |
| `assembliesToIgnore` (`string[]`) | Assembly **names** (not file names) that should be filtered out before `GetDependencies()` returns. |
| `assembliesToIgnore` (`Assembly[]`) | Convenience overload — each `Assembly`'s simple name is used. Useful when you have a typed handle and don't want to spell the name twice. |

When `assembliesToIgnore` is `null`, the standard ignore list is used.

## Standard ignore list

Without a custom list, `GetStandardAssembliesToIgnore()` is applied. Today it filters:

- `netstandard`
- `nCrunch.TestRuntime.DotNetCore`

You can read the current list explicitly:

```csharp
foreach (var name in DependencyTestBase.GetStandardAssembliesToIgnore())
{
    Console.WriteLine(name);
}
```

If you need something different, pass your own array to the constructor — the standard list is only a fallback, it's not merged with whatever you supply.

## Common patterns

**"Project X must not reference EntityFramework"** — `Assert.DoesNotContain(GetDependencies(), d => d.Name.StartsWith("EntityFramework"));`

**"Project X must only reference these assemblies"** — keep an `allowed[]` array of name prefixes and assert each dependency starts with one of them. A new transitive reference becomes a test failure rather than a runtime surprise.

**"Project X must reference version Y of package Z"** — assert against `.Version` directly: `Assert.Contains(GetDependencies(), d => d.Name == "Newtonsoft.Json" && d.Version >= new Version(13, 0, 0));`.
