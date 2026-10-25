using System.Collections;

namespace Tharga.Test.Toolkit.Tests;

public class CollectionMemberTests
{
    private sealed class LineCollection : List<OrderLine>
    {
        public string Title { get; init; }
    }

    private sealed class Batch(params OrderLine[] lines) : IEnumerable<OrderLine>
    {
        public string Name { get; init; }

        public IEnumerator<OrderLine> GetEnumerator() => ((IEnumerable<OrderLine>)lines).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static TheoryData<object> AssignedCollections => new()
    {
        new List<OrderLine>(),
        Array.Empty<int>(),
        new[] { AssignedModels.Line() },
        new List<OrderLine> { AssignedModels.Line() },
        new Dictionary<string, Customer> { ["ada"] = AssignedModels.Customer() }
    };

    [Fact]
    public void A_member_declared_on_a_list_type_is_not_checked()
    {
        var lines = new LineCollection { AssignedModels.Line() };

        Assert.Null(lines.Title);
        Assert.True(lines.IsAssigned());
    }

    [Fact]
    public void A_member_declared_on_a_custom_enumerable_is_not_checked()
    {
        var batch = new Batch(AssignedModels.Line()) { Name = null };

        Assert.True(batch.IsAssigned());
    }

    [Fact]
    public void The_elements_of_a_custom_collection_type_are_still_checked()
    {
        var lines = new LineCollection { AssignedModels.Line(), new OrderLine { Sku = "B-2" } };

        var issue = Assert.Single(lines.AssignmentIssues());

        Assert.Equal(1, issue.Index);
        Assert.Equal("Value of type Int32 in 'LineCollection.OrderLine.Quantity' is not assigned.", issue.Message);
    }

    [Theory]
    [MemberData(nameof(AssignedCollections))]
    public void The_own_properties_of_a_collection_are_not_checked(object collection)
    {
        Assert.True(collection.IsAssigned());
    }

    [Fact]
    public void An_empty_collection_member_is_assigned()
    {
        Assert.True(AssignedModels.Order().IsAssigned());
    }
}
