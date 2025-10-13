using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.Queries.Tests;

public class IQueryTests
{
    [Fact]
    public void IQuery_ShouldImplementMediatRRequest()
    {
        // Assert
        typeof(IRequest<string>).IsAssignableFrom(typeof(IQuery<string>)).ShouldBeTrue();
    }

    [Fact]
    public void ConcreteQuery_ShouldImplementIQuery()
    {
        // Arrange & Act
        var query = new TestQuery();

        // Assert
        query.ShouldBeAssignableTo<IQuery<string>>();
        query.ShouldBeAssignableTo<IRequest<string>>();
    }

    [Fact]
    public void IQuery_ShouldBeMarkerInterface()
    {
        // Assert - IQuery should have no methods or properties
        var methods = typeof(IQuery<>).GetMethods();
        var properties = typeof(IQuery<>).GetProperties();

        // Only inherited methods from IRequest should exist
        methods.Length.ShouldBe(0);
        properties.Length.ShouldBe(0);
    }

    [Fact]
    public void IQuery_ShouldSupportDifferentResponseTypes()
    {
        // Arrange & Act
        var stringQuery = new TestQuery();
        var intQuery = new TestIntQuery();
        var complexQuery = new TestComplexQuery();

        // Assert
        stringQuery.ShouldBeAssignableTo<IQuery<string>>();
        intQuery.ShouldBeAssignableTo<IQuery<int>>();
        complexQuery.ShouldBeAssignableTo<IQuery<TestComplexResult>>();
    }

    [Fact]
    public void MultipleQueries_ShouldBeDistinct()
    {
        // Arrange & Act
        var query1 = new TestQuery();
        var query2 = new TestQuery();

        // Assert
        query1.ShouldNotBeSameAs(query2);
    }

    [Fact]
    public void Query_WithProperties_ShouldStoreData()
    {
        // Arrange & Act
        var query = new TestQueryWithProperties
        {
            Id = 123,
            SearchTerm = "test"
        };

        // Assert
        query.Id.ShouldBe(123);
        query.SearchTerm.ShouldBe("test");
    }

    [Fact]
    public void Query_ShouldBeReadOnly_Conceptually()
    {
        // This test demonstrates that queries represent read operations
        // and should not modify state
        
        // Arrange & Act
        var query = new TestQuery();

        // Assert
        query.ShouldBeAssignableTo<IQuery<string>>();
        // Queries should be immutable or represent read-only operations
    }

    private class TestQuery : IQuery<string>
    {
    }

    private class TestIntQuery : IQuery<int>
    {
    }

    private class TestComplexQuery : IQuery<TestComplexResult>
    {
    }

    private class TestQueryWithProperties : IQuery<string>
    {
        public int Id { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }

    private class TestComplexResult
    {
        public int Count { get; set; }
        public List<string> Items { get; set; } = new();
    }
}
