namespace Tharga.Test.Toolkit.Tests;

public class AssignmentIssueTests
{
    [Fact]
    public void A_missing_object_name_becomes_not_available()
    {
        var issue = new AssignmentIssue(null, "message", null);

        Assert.Equal("N/A", issue.ObjectName);
        Assert.Equal("message", issue.Message);
        Assert.Null(issue.Index);
    }

    [Fact]
    public void An_index_fills_the_array_brackets_in_the_object_name()
    {
        var issue = new AssignmentIssue("Int32[].Int32", "message", 2);

        Assert.Equal("Int32[2].Int32", issue.ObjectName);
        Assert.Equal(2, issue.Index);
    }

    [Fact]
    public void Without_an_index_the_object_name_is_kept()
    {
        var issue = new AssignmentIssue("Int32[].Int32", "message", null);

        Assert.Equal("Int32[].Int32", issue.ObjectName);
    }
}
