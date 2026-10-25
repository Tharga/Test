using System.Numerics;

namespace Tharga.Test.Toolkit.Tests;

public class ScalarValueTests
{
    private static readonly Guid Key = new("6f9619ff-8b86-d011-b42d-00c04fc964ff");

    public static TheoryData<object> ScalarDefaults => new()
    {
        0L,
        0f,
        (short)0,
        (byte)0,
        (sbyte)0,
        (ushort)0,
        0u,
        0ul,
        '\0',
        Guid.Empty,
        default(DateTimeOffset),
        default(DateOnly),
        default(TimeOnly),
        default(Half),
        Int128.Zero,
        UInt128.Zero,
        nint.Zero,
        nuint.Zero,
        BigInteger.Zero,
        Complex.Zero
    };

    public static TheoryData<object> ScalarValues => new()
    {
        5L,
        0.5f,
        (short)1,
        (byte)1,
        (sbyte)-1,
        (ushort)1,
        1u,
        1ul,
        'a',
        Key,
        new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero),
        new DateOnly(2026, 10, 25),
        new TimeOnly(12, 0),
        (Half)1,
        Int128.One,
        UInt128.One,
        (nint)1,
        (nuint)1,
        new BigInteger(5),
        new Complex(1, 0)
    };

    [Theory]
    [MemberData(nameof(ScalarDefaults))]
    public void A_scalar_holding_its_default_is_not_assigned(object value)
    {
        Assert.False(value.IsAssigned());
    }

    [Theory]
    [MemberData(nameof(ScalarValues))]
    public void A_scalar_holding_a_value_is_assigned(object value)
    {
        Assert.True(value.IsAssigned());
    }

    [Fact]
    public void Every_default_scalar_member_is_reported()
    {
        var issues = new Scalars().AssignmentIssues().Select(x => x.Message).Order().ToArray();

        string[] expected =
        [
            "Value of type BigInteger in 'Scalars.Large' is not assigned.",
            "Value of type Char in 'Scalars.Initial' is not assigned.",
            "Value of type DateOnly in 'Scalars.Born' is not assigned.",
            "Value of type DateTimeOffset in 'Scalars.Created' is not assigned.",
            "Value of type Guid in 'Scalars.Key' is not assigned.",
            "Value of type Int64 in 'Scalars.Total' is not assigned.",
            "Value of type Single in 'Scalars.Weight' is not assigned.",
            "Value of type TimeOnly in 'Scalars.Opens' is not assigned."
        ];
        Assert.Equal(expected.Order(), issues);
    }

    [Fact]
    public void Scalar_members_holding_values_are_assigned()
    {
        var scalars = new Scalars
        {
            Total = 5,
            Weight = 0.5f,
            Initial = 'a',
            Key = Key,
            Created = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero),
            Born = new DateOnly(2026, 10, 25),
            Opens = new TimeOnly(12, 0),
            Large = new BigInteger(5)
        };

        Assert.Empty(scalars.AssignmentIssues());
    }

    [Fact]
    public void A_scalar_member_issue_names_the_full_member_path()
    {
        var scalars = new Scalars
        {
            Total = 5,
            Weight = 0.5f,
            Initial = 'a',
            Created = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero),
            Born = new DateOnly(2026, 10, 25),
            Opens = new TimeOnly(12, 0),
            Large = new BigInteger(5)
        };

        var issue = Assert.Single(scalars.AssignmentIssues());

        Assert.Equal("Scalars.Key", issue.ObjectName);
        Assert.Equal("Value of type Guid in 'Scalars.Key' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_nullable_scalar_holding_its_default_is_compared_as_the_underlying_type()
    {
        var issues = new NullableScalars { Total = 0, Key = Guid.Empty }.AssignmentIssues().Select(x => x.Message).Order().ToArray();

        string[] expected =
        [
            "Value of type Guid in 'NullableScalars.Key' is not assigned.",
            "Value of type Int64 in 'NullableScalars.Total' is not assigned."
        ];
        Assert.Equal(expected.Order(), issues);
    }

    [Fact]
    public void A_null_nullable_scalar_is_reported_as_a_null_member()
    {
        var issue = Assert.Single(new NullableScalars { Total = null, Key = Key }.AssignmentIssues());

        Assert.Equal("NullableScalars", issue.ObjectName);
        Assert.Equal("The field 'NullableScalars.Total' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_nullable_scalar_holding_a_value_is_assigned()
    {
        Assert.True(new NullableScalars { Total = 5, Key = Key }.IsAssigned());
    }

    [Fact]
    public void A_user_defined_struct_is_walked_and_its_scalar_members_are_compared()
    {
        var reading = new Reading { Value = new Measurement { Amount = 3 } };

        var issue = Assert.Single(reading.AssignmentIssues());

        Assert.Equal("Reading.Value.Unit", issue.ObjectName);
        Assert.Equal("Value of type Guid in 'Reading.Value.Unit' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_default_user_defined_struct_reports_each_member_rather_than_the_struct()
    {
        var issues = new Reading().AssignmentIssues().Select(x => x.ObjectName).Order().ToArray();

        Assert.Equal(new[] { "Reading.Value.Amount", "Reading.Value.Unit" }, issues);
    }

    [Fact]
    public void A_generic_struct_is_walked_member_by_member()
    {
        var pair = new Pair { Value = (1, null) };

        var issue = Assert.Single(pair.AssignmentIssues());

        Assert.Equal("Pair.Value", issue.ObjectName);
        Assert.Equal("The field 'Pair.Value.Item2' is not assigned.", issue.Message);
    }
}
