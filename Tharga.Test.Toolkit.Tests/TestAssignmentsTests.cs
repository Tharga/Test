using System.Reflection;

namespace Tharga.Test.Toolkit.Tests;

public class TestAssignmentsTests
{
    private const int SpecimenNumber = 7;

    private static object Specimen(Type type) => type == typeof(int) ? SpecimenNumber : null;

    internal static class StaticConverter
    {
        public static string Describe(int number) => $"#{number}";

        public static int Twice(int number) => number * 2;

        internal static string Hidden() => "hidden";
    }

    internal sealed class InstanceConverter(string prefix)
    {
        public string Format(int number) => $"{prefix}{number}";

        public static string Shared() => "shared";
    }

    [Fact]
    public void The_type_overload_invokes_every_public_static_method()
    {
        var results = typeof(StaticConverter).TestAssignments(Specimen).ToDictionary(x => x.method, x => x.data);

        Assert.Equal(2, results.Count);
        Assert.Equal("#7", results["Describe"]);
        Assert.Equal(14, results["Twice"]);
    }

    [Fact]
    public void The_type_overload_does_not_invoke_instance_methods()
    {
        var results = typeof(InstanceConverter).TestAssignments(Specimen).ToArray();

        var (method, data) = Assert.Single(results);
        Assert.Equal("Shared", method);
        Assert.Equal("shared", data);
    }

    [Fact]
    public void The_type_overload_skips_ignored_functions()
    {
        var results = typeof(StaticConverter).TestAssignments(Specimen, ["Twice"]).ToArray();

        var (method, _) = Assert.Single(results);
        Assert.Equal("Describe", method);
    }

    [Fact]
    public void The_type_overload_honours_the_binding_flags()
    {
        var results = typeof(StaticConverter).TestAssignments(Specimen, bindingAttr: BindingFlags.Static | BindingFlags.NonPublic).ToArray();

        var (method, data) = Assert.Single(results);
        Assert.Equal("Hidden", method);
        Assert.Equal("hidden", data);
    }

    [Fact]
    public void The_specimen_factory_is_asked_for_every_parameter_type()
    {
        var requested = new List<Type>();

        _ = typeof(StaticConverter).TestAssignments(type =>
        {
            requested.Add(type);
            return Specimen(type);
        }).ToArray();

        Assert.Equal(new[] { typeof(int), typeof(int) }, requested);
    }

    [Fact]
    public void The_instance_overload_invokes_public_instance_methods_except_GetType_and_Equals()
    {
        var results = new InstanceConverter("x").TestAssignments(Specimen).ToDictionary(x => x.method, x => x.data);

        Assert.Equal(new[] { "Format", "GetHashCode", "ToString" }.Order(), results.Keys.Order());
        Assert.Equal("x7", results["Format"]);
    }

    [Fact]
    public void The_instance_overload_skips_ignored_functions_as_well_as_the_standard_ones()
    {
        var results = new InstanceConverter("x").TestAssignments(Specimen, ["ToString", "GetHashCode"]).ToArray();

        var (method, data) = Assert.Single(results);
        Assert.Equal("Format", method);
        Assert.Equal("x7", data);
    }

    [Fact]
    public void Methods_are_invoked_only_when_the_results_are_enumerated()
    {
        var calls = 0;

        var results = typeof(StaticConverter).TestAssignments(type =>
        {
            calls++;
            return Specimen(type);
        });

        Assert.Equal(0, calls);
        _ = results.ToArray();
        Assert.Equal(2, calls);
    }
}
