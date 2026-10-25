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
    public void A_cycle_of_records_with_value_equality_terminates()
    {
        var first = new Link();
        var second = new Link { Next = first };
        first.Next = second;

        var messages = first.AssignmentIssues().Select(x => x.Message).Order().ToArray();

        string[] expected =
        [
            "The field 'Link.Name' is not assigned.",
            "The field 'Link.Next.Name' is not assigned."
        ];
        Assert.Equal(expected.Order(), messages);
    }
}
