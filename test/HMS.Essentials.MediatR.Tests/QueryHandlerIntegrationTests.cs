using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.Tests;

public class QueryHandlerIntegrationTests
{
    [Fact]
    public void IQueryHandler_ShouldImplementMediatRInterface()
    {
        // Assert
        typeof(IRequestHandler<TestQuery, string>)
            .IsAssignableFrom(typeof(IQueryHandler<TestQuery, string>)).ShouldBeTrue();
    }

    [Fact]
    public async Task QueryHandler_ShouldExecuteCorrectly()
    {
        // Arrange
        var handler = new TestQueryHandler();
        var query = new TestQuery { Id = 123 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBe("Result for ID: 123");
    }

    [Fact]
    public async Task QueryHandler_WithComplexType_ShouldWorkCorrectly()
    {
        // Arrange
        var handler = new TestComplexQueryHandler();
        var query = new TestComplexQuery { SearchTerm = "test" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.Items.ShouldContain(item => item.Name == "Test Item 1");
        result.Items.ShouldContain(item => item.Name == "Test Item 2");
    }

    private class TestQuery : IQuery<string>
    {
        public int Id { get; set; }
    }

    private class TestComplexQuery : IQuery<TestComplexResult>
    {
        public string SearchTerm { get; set; } = string.Empty;
    }

    private class TestComplexResult
    {
        public int Count { get; set; }
        public List<TestItem> Items { get; set; } = new();
    }

    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<string> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Result for ID: {request.Id}");
        }
    }

    private class TestComplexQueryHandler : IQueryHandler<TestComplexQuery, TestComplexResult>
    {
        public Task<TestComplexResult> Handle(TestComplexQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestComplexResult
            {
                Count = 2,
                Items = new List<TestItem>
                {
                    new TestItem { Id = 1, Name = "Test Item 1" },
                    new TestItem { Id = 2, Name = "Test Item 2" }
                }
            });
        }
    }
}
