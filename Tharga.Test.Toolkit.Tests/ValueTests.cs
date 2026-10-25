namespace Tharga.Test.Toolkit.Tests;

public class ValueTests
{
    public static TheoryData<object> DefaultValues => new()
    {
        0,
        0m,
        0d,
        false,
        default(DateTime),
        TimeSpan.Zero,
        Status.None
    };

    public static TheoryData<object> NonDefaultValues => new()
    {
        1,
        1.5m,
        0.5d,
        true,
        new DateTime(2026, 1, 1),
        TimeSpan.FromSeconds(1),
        Status.Active
    };

    [Theory]
    [MemberData(nameof(DefaultValues))]
    public void A_default_value_is_not_assigned(object value)
    {
        Assert.False(value.IsAssigned());
    }

    [Theory]
    [MemberData(nameof(NonDefaultValues))]
    public void A_non_default_value_is_assigned(object value)
    {
        Assert.True(value.IsAssigned());
    }

    [Fact]
    public void Every_default_member_is_reported()
    {
        var issues = new Values().AssignmentIssues().Select(x => x.Message).Order().ToArray();

        string[] expected =
        [
            "The field 'Values.Link' is not assigned.",
            "Value of type Boolean in 'Values.Flag' is not assigned.",
            "Value of type DateTime in 'Values.When' is not assigned.",
            "Value of type Decimal in 'Values.Amount' is not assigned.",
            "Value of type Double in 'Values.Ratio' is not assigned.",
            "Value of type Int32 in 'Values.Number' is not assigned.",
            "Value of type Status in 'Values.Status' is not assigned.",
            "Value of type TimeSpan in 'Values.Duration' is not assigned."
        ];
        Assert.Equal(expected.Order(), issues);
    }

    [Fact]
    public void Members_holding_non_default_values_are_assigned()
    {
        var values = new Values
        {
            Number = 1,
            Amount = 2m,
            Ratio = 0.5d,
            Flag = true,
            When = new DateTime(2026, 1, 1),
            Duration = TimeSpan.FromMinutes(1),
            Status = Status.Active,
            Link = new Uri("https://example.com")
        };

        Assert.True(values.IsAssigned());
        Assert.Empty(values.AssignmentIssues());
    }

    [Fact]
    public void A_value_member_issue_names_the_full_member_path()
    {
        var issue = Assert.Single(new Customer { Name = "Ada" }.AssignmentIssues());

        Assert.Equal("Customer.Id", issue.ObjectName);
        Assert.Equal("Value of type Int32 in 'Customer.Id' is not assigned.", issue.Message);
        Assert.Null(issue.Index);
    }

    [Fact]
    public void A_nullable_member_without_a_value_is_not_assigned()
    {
        Assert.False(new Counter { Count = null }.IsAssigned());
    }

    [Fact]
    public void A_nullable_member_holding_the_default_value_is_not_assigned()
    {
        Assert.False(new Counter { Count = 0 }.IsAssigned());
    }

    [Fact]
    public void A_nullable_member_holding_a_value_is_assigned()
    {
        Assert.True(new Counter { Count = 3 }.IsAssigned());
    }

    [Fact]
    public void A_struct_member_is_checked_member_by_member()
    {
        var shape = new Shape { Name = "Square", Location = new Point { X = 1, Y = 0 } };

        var issue = Assert.Single(shape.AssignmentIssues());

        Assert.Equal("Shape.Location.Y", issue.ObjectName);
        Assert.Equal("Value of type Int32 in 'Shape.Location.Y' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_fully_assigned_struct_member_is_assigned()
    {
        var shape = new Shape { Name = "Square", Location = new Point { X = 1, Y = 2 } };

        Assert.True(shape.IsAssigned());
    }
}
