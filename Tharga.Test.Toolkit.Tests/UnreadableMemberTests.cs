namespace Tharga.Test.Toolkit.Tests;

public class UnreadableMemberTests
{
    [Fact]
    public void A_getter_that_throws_is_reported()
    {
        var issue = Assert.Single(new Faulty { Name = "x" }.AssignmentIssues());

        Assert.Equal("The property 'Faulty.Broken' could not be read: Not available in this state.", issue.Message);
        Assert.False(new Faulty { Name = "x" }.IsAssigned());
    }

    [Fact]
    public void An_ignored_getter_that_throws_is_not_reported()
    {
        Assert.True(new Faulty { Name = "x" }.IsAssigned(["Faulty.Broken"]));
    }

    [Fact]
    public void An_indexer_is_not_read()
    {
        var issue = Assert.Single(new Shelf().AssignmentIssues());

        Assert.Equal("The field 'Shelf.Name' is not assigned.", issue.Message);
        Assert.True(new Shelf { Name = "top" }.IsAssigned());
    }

    [Fact]
    public void An_ImmutableArray_member_returns_a_result()
    {
        AssertReturnsAResult(new Batch { Items = [1, 2] });
    }

    [Fact]
    public void An_ArraySegment_member_returns_a_result()
    {
        AssertReturnsAResult(new Window { Part = new ArraySegment<int>([1, 2, 3], 1, 2) });
    }

    [Fact]
    public void A_ReadOnlyMemory_member_returns_a_result()
    {
        AssertReturnsAResult(new Payload { Data = new byte[] { 1, 2 } });
    }

    private static void AssertReturnsAResult(object item)
    {
        Assert.Null(Record.Exception(() => item.AssignmentIssues().ToArray()));
        Assert.Null(Record.Exception(() => item.IsAssigned()));
    }
}
