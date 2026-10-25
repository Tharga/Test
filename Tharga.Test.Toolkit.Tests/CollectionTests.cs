namespace Tharga.Test.Toolkit.Tests;

public class CollectionTests
{
    [Fact]
    public void A_collection_whose_elements_are_assigned_is_assigned()
    {
        var order = AssignedModels.Order(AssignedModels.Line("A-1"), AssignedModels.Line("B-2"));

        Assert.True(order.IsAssigned());
    }

    [Fact]
    public void Each_collection_element_is_checked_and_carries_its_index()
    {
        var order = AssignedModels.Order(AssignedModels.Line(), new OrderLine { Sku = null, Quantity = 2 });

        var issue = Assert.Single(order.AssignmentIssues());

        Assert.Equal(1, issue.Index);
        Assert.Equal("Order.Lines.OrderLine", issue.ObjectName);
        Assert.Equal("The field 'Order.Lines.OrderLine.Sku' is not assigned.", issue.Message);
    }

    [Fact]
    public void Every_unassigned_element_is_reported()
    {
        var order = AssignedModels.Order(new OrderLine { Sku = "A-1" }, AssignedModels.Line(), new OrderLine { Sku = "C-3" });

        var indexes = order.AssignmentIssues().Select(x => x.Index).ToArray();

        Assert.Equal(new int?[] { 0, 2 }, indexes);
    }

    [Fact]
    public void A_null_collection_is_reported()
    {
        var order = new Order { Customer = AssignedModels.Customer(), Lines = null };

        var issue = Assert.Single(order.AssignmentIssues());

        Assert.Equal("The field 'Order.Lines' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_null_element_is_reported_with_its_index()
    {
        var order = AssignedModels.Order(AssignedModels.Line(), null);

        var issue = Assert.Single(order.AssignmentIssues());

        Assert.Equal(1, issue.Index);
        Assert.Equal("Order.Lines", issue.ObjectName);
    }

    [Fact]
    public void A_default_value_element_is_reported_with_its_index()
    {
        var issue = Assert.Single(new[] { 1, 0, 3 }.AssignmentIssues());

        Assert.Equal(1, issue.Index);
        Assert.Equal("Int32[1].Int32", issue.ObjectName);
    }

    [Fact]
    public void Dictionary_values_are_checked()
    {
        var map = new Dictionary<string, Customer>
        {
            ["ada"] = AssignedModels.Customer(),
            ["bob"] = new Customer { Id = 2 }
        };

        var issue = Assert.Single(map.AssignmentIssues());

        Assert.Equal(1, issue.Index);
        Assert.EndsWith(".Value.Name' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_string_is_not_walked_as_a_collection_of_characters()
    {
        Assert.True("abc".IsAssigned());
    }
}
