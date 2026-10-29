namespace Tharga.Test.Toolkit.Tests;

public class ReflectionMemberTests
{
    [Fact]
    public void A_Type_member_counts_as_assigned_when_set()
    {
        var holder = new TypeHolder { Kind = typeof(Order) };

        Assert.Empty(holder.AssignmentIssues());
        Assert.True(holder.IsAssigned());
    }

    [Fact]
    public void A_Type_member_is_reported_when_null()
    {
        var issue = Assert.Single(new TypeHolder().AssignmentIssues());

        Assert.Equal("The field 'TypeHolder.Kind' is not assigned.", issue.Message);
    }

    [Fact]
    public void Other_reflection_members_count_as_assigned_when_set()
    {
        var holder = new ReflectionHolder { Method = typeof(Order).GetMethod(nameof(ToString)), Assembly = typeof(Order).Assembly };

        Assert.Empty(holder.AssignmentIssues());
    }
}
