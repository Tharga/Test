namespace Tharga.Test.Toolkit.Tests;

public class ReferenceTypeTests
{
    [Fact]
    public void Null_is_not_assigned()
    {
        object value = null;

        Assert.False(value.IsAssigned());
        var issue = Assert.Single(value.AssignmentIssues());
        Assert.Equal("N/A", issue.ObjectName);
    }

    [Fact]
    public void A_fully_assigned_object_is_assigned()
    {
        var customer = AssignedModels.Customer();

        Assert.True(customer.IsAssigned());
        Assert.Empty(customer.AssignmentIssues());
    }

    [Fact]
    public void A_null_string_member_is_reported_on_its_parent()
    {
        var issue = Assert.Single(new Customer { Id = 7 }.AssignmentIssues());

        Assert.Equal("Customer", issue.ObjectName);
        Assert.Equal("The field 'Customer.Name' is not assigned.", issue.Message);
        Assert.Null(issue.Index);
    }

    [Fact]
    public void A_null_member_is_named_by_its_parent_and_a_default_value_by_its_own_path()
    {
        var issues = new Customer { Id = 0, Name = null }.AssignmentIssues()
            .Select(x => (x.ObjectName, x.Message))
            .Order()
            .ToArray();

        (string, string)[] expected =
        [
            ("Customer", "The field 'Customer.Name' is not assigned."),
            ("Customer.Id", "Value of type Int32 in 'Customer.Id' is not assigned.")
        ];
        Assert.Equal(expected, issues);
    }

    [Fact]
    public void An_empty_string_member_is_not_reported()
    {
        Assert.True(new Customer { Id = 7, Name = string.Empty }.IsAssigned());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void A_non_null_string_is_assigned_even_when_empty(string value)
    {
        Assert.True(value.IsAssigned());
        Assert.Empty(new FieldHolder { Label = value }.AssignmentIssues());
    }

    [Fact]
    public void A_null_uri_member_is_reported()
    {
        var values = new Values
        {
            Number = 1,
            Amount = 1m,
            Ratio = 1d,
            Flag = true,
            When = new DateTime(2026, 1, 1),
            Duration = TimeSpan.FromSeconds(1),
            Status = Status.Active,
            Link = null
        };

        var issue = Assert.Single(values.AssignmentIssues());

        Assert.Equal("The field 'Values.Link' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_public_field_is_checked()
    {
        var issue = Assert.Single(new FieldHolder().AssignmentIssues());

        Assert.Equal("The field 'FieldHolder.Label' is not assigned.", issue.Message);
        Assert.True(new FieldHolder { Label = "set" }.IsAssigned());
    }

    [Fact]
    public void A_null_nested_reference_is_reported()
    {
        var order = new Order { Customer = null, Lines = [AssignedModels.Line()] };

        var issue = Assert.Single(order.AssignmentIssues());

        Assert.Equal("Order", issue.ObjectName);
        Assert.Equal("The field 'Order.Customer' is not assigned.", issue.Message);
    }

    [Fact]
    public void The_members_of_a_nested_reference_are_checked()
    {
        var order = new Order { Customer = new Customer { Id = 7 }, Lines = [AssignedModels.Line()] };

        var issue = Assert.Single(order.AssignmentIssues());

        Assert.Equal("Order.Customer", issue.ObjectName);
        Assert.Equal("The field 'Order.Customer.Name' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_fully_assigned_graph_is_assigned()
    {
        Assert.True(AssignedModels.Order(AssignedModels.Line()).IsAssigned());
    }

    [Fact]
    public void A_cyclic_graph_is_walked_once_per_instance()
    {
        var root = new Node { Name = "root" };
        root.Child = new Node { Name = "child", Parent = root };

        var messages = root.AssignmentIssues().Select(x => x.Message).Order().ToArray();

        string[] expected =
        [
            "The field 'Node.Child.Child' is not assigned.",
            "The field 'Node.Parent' is not assigned."
        ];
        Assert.Equal(expected.Order(), messages);
    }
}
