namespace Tharga.Test.Toolkit.Tests;

public class StaticMemberTests
{
    [Fact]
    public void A_null_static_member_is_not_reported()
    {
        Assert.Null(SharedState.Shared);
        Assert.Null(SharedState.SharedField);

        var issue = Assert.Single(new SharedState().AssignmentIssues());

        Assert.Equal("The field 'SharedState.Name' is not assigned.", issue.Message);
    }

    [Fact]
    public void A_static_property_that_returns_a_new_instance_does_not_recurse()
    {
        var issue = Assert.Single(new Defaulted().AssignmentIssues());

        Assert.Equal("The field 'Defaulted.Name' is not assigned.", issue.Message);
        Assert.True(new Defaulted { Name = "set" }.IsAssigned());
    }

    [Fact]
    public void Static_members_of_a_user_struct_are_not_reported()
    {
        var issue = Assert.Single(new Marker { Position = new Spot { X = 1 } }.AssignmentIssues());

        Assert.Equal("Marker.Position.Y", issue.ObjectName);
        Assert.True(new Marker { Position = new Spot { X = 1, Y = 2 } }.IsAssigned());
    }

    [Fact]
    public void A_non_public_instance_member_is_not_reported()
    {
        var issue = Assert.Single(new Restricted().AssignmentIssues());

        Assert.Equal("The field 'Restricted.Name' is not assigned.", issue.Message);
    }
}
