using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.Queries.Tests;

public class QueryBaseTests
{
    [Fact]
    public void QueryBase_ShouldImplementIQuery()
    {
        // Arrange & Act
        var query = new TestQueryBase();

        // Assert
        query.ShouldBeAssignableTo<QueryBase<string>>();
        query.ShouldBeAssignableTo<IQuery<string>>();
        query.ShouldBeAssignableTo<IRequest<string>>();
    }

    [Fact]
    public void QueryBase_ShouldSetCreatedAtOnCreation()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var query = new TestQueryBase();

        // Assert
        var afterCreation = DateTimeOffset.UtcNow;
        query.CreatedAt.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        query.CreatedAt.ShouldBeLessThanOrEqualTo(afterCreation);
    }

    [Fact]
    public void QueryBase_CreatedAt_ShouldBeReadOnly()
    {
        // Arrange & Act
        var query = new TestQueryBase();
        var initialCreatedAt = query.CreatedAt;

        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Assert - CreatedAt should not change
        query.CreatedAt.ShouldBe(initialCreatedAt);
    }

    [Fact]
    public void QueryBase_WithProperties_ShouldWorkAsRecord()
    {
        // Arrange & Act
        var query1 = new TestQueryWithProperties("test", 123);
        var query2 = new TestQueryWithProperties("test", 123);

        // Assert - Records with same values should be equal (except CreatedAt)
        query1.SearchTerm.ShouldBe(query2.SearchTerm);
        query1.PageSize.ShouldBe(query2.PageSize);
    }

    [Fact]
    public void QueryBase_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange & Act
        var query1 = new TestQueryWithProperties("test1", 10);
        var query2 = new TestQueryWithProperties("test2", 20);

        // Assert
        query1.ShouldNotBe(query2);
        query1.SearchTerm.ShouldNotBe(query2.SearchTerm);
        query1.PageSize.ShouldNotBe(query2.PageSize);
    }

    [Fact]
    public void QueryBase_ShouldSupportDifferentResponseTypes()
    {
        // Arrange & Act
        var stringQuery = new TestQueryBase();
        var intQuery = new TestIntQueryBase();
        var complexQuery = new TestComplexQueryBase();

        // Assert
        stringQuery.ShouldBeAssignableTo<QueryBase<string>>();
        intQuery.ShouldBeAssignableTo<QueryBase<int>>();
        complexQuery.ShouldBeAssignableTo<QueryBase<TestComplexResult>>();
    }

    [Fact]
    public void MultipleQueries_ShouldHaveDifferentCreationTimes()
    {
        // Arrange & Act
        var query1 = new TestQueryBase();
        Thread.Sleep(10);
        var query2 = new TestQueryBase();

        // Assert
        query2.CreatedAt.ShouldBeGreaterThan(query1.CreatedAt);
    }

    [Fact]
    public void QueryBase_ShouldBeRecord()
    {
        // Arrange & Act
        var query = new TestQueryWithProperties("test", 10);
        var modifiedQuery = query with { SearchTerm = "modified" };

        // Assert
        modifiedQuery.SearchTerm.ShouldBe("modified");
        modifiedQuery.PageSize.ShouldBe(10);
        query.SearchTerm.ShouldBe("test"); // Original should be unchanged
    }

    [Fact]
    public void QueryBase_WithComplexProperties_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var query = new TestComplexQueryWithProperties(
            new List<string> { "filter1", "filter2" },
            new Dictionary<string, string> { { "key1", "value1" } }
        );

        // Assert
        query.Filters.Count.ShouldBe(2);
        query.Filters.ShouldContain("filter1");
        query.Filters.ShouldContain("filter2");
        query.Metadata.Count.ShouldBe(1);
        query.Metadata["key1"].ShouldBe("value1");
    }

    [Fact]
    public void QueryBase_ShouldSupportPagination()
    {
        // Arrange & Act
        var query = new TestPaginatedQuery(1, 20, "name");

        // Assert
        query.PageNumber.ShouldBe(1);
        query.PageSize.ShouldBe(20);
        query.OrderBy.ShouldBe("name");
    }

    private record TestQueryBase : QueryBase<string>
    {
    }

    private record TestIntQueryBase : QueryBase<int>
    {
    }

    private record TestComplexQueryBase : QueryBase<TestComplexResult>
    {
    }

    private record TestQueryWithProperties(string SearchTerm, int PageSize) : QueryBase<string>
    {
    }

    private record TestComplexQueryWithProperties(
        List<string> Filters,
        Dictionary<string, string> Metadata
    ) : QueryBase<List<string>>
    {
    }

    private record TestPaginatedQuery(
        int PageNumber,
        int PageSize,
        string OrderBy
    ) : QueryBase<TestPaginatedResult>
    {
    }

    private class TestComplexResult
    {
        public int Count { get; set; }
        public List<string> Items { get; set; } = new();
    }

    private class TestPaginatedResult
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<string> Items { get; set; } = new();
    }
}
