namespace Tharga.Test.Toolkit.Tests;

public class IgnorePropertiesTests
{
    [Fact]
    public void An_ignored_value_member_is_not_reported()
    {
        var settings = new Settings { Host = "api.example.com", UseTls = false };

        Assert.False(settings.IsAssigned());
        Assert.True(settings.IsAssigned(["Settings.UseTls"]));
        Assert.Empty(settings.AssignmentIssues(["Settings.UseTls"]));
    }

    [Fact]
    public void An_ignored_null_member_is_not_reported()
    {
        var customer = new Customer { Id = 7, Name = null };

        Assert.True(customer.IsAssigned(["Customer.Name"]));
    }

    [Fact]
    public void Ignoring_one_member_still_reports_the_others()
    {
        var issue = Assert.Single(new Customer().AssignmentIssues(["Customer.Name"]));

        Assert.Equal("Customer.Id", issue.ObjectName);
    }

    [Fact]
    public void A_nested_member_is_ignored_by_its_full_path_only()
    {
        var order = new Order { Customer = new Customer { Id = 7 }, Lines = [AssignedModels.Line()] };

        Assert.True(order.IsAssigned(["Order.Customer.Name"]));
        Assert.False(order.IsAssigned(["Customer.Name"]));
    }

    [Fact]
    public void A_collection_element_member_is_ignored_by_its_element_path()
    {
        var order = AssignedModels.Order(AssignedModels.Line(), new OrderLine { Sku = null, Quantity = 1 });

        Assert.True(order.IsAssigned(["Order.Lines.OrderLine.Sku"]));
    }

    [Fact]
    public void Ignoring_a_reference_member_does_not_skip_its_members()
    {
        var order = new Order { Customer = new Customer { Id = 7 }, Lines = [AssignedModels.Line()] };

        var issue = Assert.Single(order.AssignmentIssues(["Order.Customer"]));

        Assert.Equal("The field 'Order.Customer.Name' is not assigned.", issue.Message);
    }
}
