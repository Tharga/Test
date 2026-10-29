namespace Tharga.Test.Toolkit.Tests;

public class ObjectIdentityTests
{
    [Fact]
    public void An_equal_record_at_a_second_path_has_its_unassigned_member_reported_under_that_path()
    {
        var contacts = new Contacts { Primary = new Contact("Ada", null), Backup = new Contact("Ada", null) };
        Assert.Equal(contacts.Primary, contacts.Backup);

        var issue = Assert.Single(contacts.AssignmentIssues(["Contacts.Primary.Email"]));

        Assert.Equal("Contacts.Backup", issue.ObjectName);
        Assert.Equal("The field 'Contacts.Backup.Email' is not assigned.", issue.Message);
    }

    [Fact]
    public void Equal_records_at_different_paths_are_each_reported()
    {
        var contacts = new Contacts { Primary = new Contact("Ada", null), Backup = new Contact("Ada", null) };

        var messages = contacts.AssignmentIssues().Select(x => x.Message).Order().ToArray();

        string[] expected =
        [
            "The field 'Contacts.Primary.Email' is not assigned.",
            "The field 'Contacts.Backup.Email' is not assigned."
        ];
        Assert.Equal(expected.Order(), messages);
    }

    [Fact]
    public void An_object_that_equals_another_by_id_has_its_own_unassigned_member_reported()
    {
        var transfer = new Transfer { From = new Account { Id = 1, Owner = "Ada" }, To = new Account { Id = 1 } };
        Assert.Equal(transfer.From, transfer.To);

        var issue = Assert.Single(transfer.AssignmentIssues());

        Assert.Equal("Transfer.To", issue.ObjectName);
        Assert.Equal("The field 'Transfer.To.Owner' is not assigned.", issue.Message);
    }

    [Fact]
    public void Equal_struct_values_at_different_paths_are_each_reported()
    {
        var issues = new Route().AssignmentIssues().Select(x => x.ObjectName).Order().ToArray();

        Assert.Equal(new[] { "Route.End.X", "Route.End.Y", "Route.Start.X", "Route.Start.Y" }, issues);
    }

    [Fact]
    public void An_object_that_references_itself_is_walked_once()
    {
        var node = new Node { Name = "self" };
        node.Child = node;

        var issue = Assert.Single(node.AssignmentIssues());

        Assert.Equal("The field 'Node.Parent' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_cycle_of_records_terminates()
    {
        var first = new Link { Name = "first" };
        var second = new Link { Next = first };
        first.Next = second;

        var issue = Assert.Single(first.AssignmentIssues());

        Assert.Equal("The field 'Link.Next.Name' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_collection_that_contains_itself_terminates()
    {
        var list = new List<object>();
        list.Add(list);

        Assert.Empty(list.AssignmentIssues());
        Assert.True(list.IsAssigned());
    }

    [Fact]
    public void A_cycle_of_collections_terminates()
    {
        var first = new List<object>();
        var second = new List<object> { first };
        first.Add(second);

        Assert.Empty(first.AssignmentIssues());
        Assert.True(first.IsAssigned());
    }

    [Fact]
    public void The_elements_of_a_collection_are_still_checked()
    {
        var list = new List<object> { AssignedModels.Line(), new OrderLine { Quantity = 2 } };

        var issue = Assert.Single(list.AssignmentIssues());

        Assert.Equal(1, issue.Index);
        Assert.Equal("The field 'List`1.OrderLine.Sku' is not assigned.", issue.Message);
        Assert.False(list.IsAssigned());
    }

    [Fact]
    public void A_struct_member_that_returns_an_equal_copy_is_not_walked_again()
    {
        var issue = Assert.Single(new Bearing { Heading = new Angle() }.AssignmentIssues());

        Assert.Equal("Bearing.Heading.Degrees", issue.ObjectName);
        Assert.True(new Bearing { Heading = new Angle { Degrees = 90 } }.IsAssigned());
    }

    [Fact]
    public void A_record_member_that_returns_an_equal_new_instance_is_not_walked_again()
    {
        var issue = Assert.Single(new Wallet { Balance = new Money(0) }.AssignmentIssues());

        Assert.Equal("Wallet.Balance.Amount", issue.ObjectName);
        Assert.True(new Wallet { Balance = new Money(-5) }.IsAssigned());
    }

    [Fact]
    public void A_value_equal_to_one_in_another_branch_is_still_walked()
    {
        var wallets = new[] { new Wallet { Balance = new Money(0) }, new Wallet { Balance = new Money(0) } };

        var issues = wallets.AssignmentIssues().Select(x => (x.ObjectName, x.Index)).ToArray();

        Assert.Equal(new[] { ("Wallet[0].Wallet.Balance.Amount", (int?)0), ("Wallet[1].Wallet.Balance.Amount", (int?)1) }, issues);
    }
}
