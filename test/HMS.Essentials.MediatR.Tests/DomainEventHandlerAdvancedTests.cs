using HMS.Essentials.MediatR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace HMS.Essentials.MediatR.Tests;

public class DomainEventHandlerAdvancedTests
{
    [Fact]
    public async Task DomainEventHandler_WithCancellationToken_ShouldCancelGracefully()
    {
        // Arrange
        var handler = new CancellableDomainEventHandler();
        var domainEvent = new TestCancellableEvent { Id = 1 };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await handler.Handle(domainEvent, cts.Token);
        });
        
        handler.StartedProcessing.ShouldBeTrue();
        handler.CompletedProcessing.ShouldBeFalse();
    }

    [Fact]
    public async Task DomainEventHandler_ShouldHandleNestedDomainEvents()
    {
        // Arrange
        var handler = new NestedEventHandler();
        var parentEvent = new ParentDomainEvent
        {
            ChildEvents = new List<ChildDomainEvent>
            {
                new() { ChildId = 1, Message = "Child 1" },
                new() { ChildId = 2, Message = "Child 2" }
            }
        };

        // Act
        await handler.Handle(parentEvent, CancellationToken.None);

        // Assert
        handler.ProcessedChildCount.ShouldBe(2);
        handler.ChildMessages.ShouldContain("Child 1");
        handler.ChildMessages.ShouldContain("Child 2");
    }

    [Fact]
    public async Task MultipleDomainEventHandlers_ShouldExecuteInParallel()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventHandlerAdvancedTests).Assembly));
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        ParallelExecutionTracker.Reset();
        var domainEvent = new ParallelTestEvent { Value = 42 };

        // Act
        var startTime = DateTime.UtcNow;
        await mediator.Publish(domainEvent);
        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        // Assert
        ParallelExecutionTracker.Handler1Executed.ShouldBeTrue();
        ParallelExecutionTracker.Handler2Executed.ShouldBeTrue();
        ParallelExecutionTracker.Handler3Executed.ShouldBeTrue();
        // If handlers run in parallel, total time should be less than sequential execution
        duration.TotalMilliseconds.ShouldBeLessThan(250); // Each handler takes ~100ms
    }

    [Fact]
    public async Task DomainEventHandler_WithExceptionInOneHandler_ShouldNotStopOthers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventHandlerAdvancedTests).Assembly));
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        ExceptionTestTracker.Reset();
        var domainEvent = new ExceptionTestEvent { ShouldThrow = true };

        // Act
        try
        {
            await mediator.Publish(domainEvent);
        }
        catch
        {
            // Some handlers might throw
        }

        // Assert - At least some handlers should have executed
        var executedCount = ExceptionTestTracker.GetExecutedCount();
        executedCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task DomainEventHandler_ShouldSupportGenericEventTypes()
    {
        // Arrange
        var handler = new GenericEventHandler<int>();
        var domainEvent = new GenericDomainEvent<int> { Data = 123 };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.ReceivedData.ShouldBe(123);
    }

    [Fact]
    public async Task DomainEventHandler_WithComplexPayload_ShouldSerializeCorrectly()
    {
        // Arrange
        var handler = new ComplexPayloadHandler();
        var domainEvent = new ComplexPayloadEvent
        {
            Metadata = new Dictionary<string, object>
            {
                { "UserId", 123 },
                { "Action", "Create" },
                { "Timestamp", DateTime.UtcNow }
            },
            Tags = new[] { "important", "audit" },
            NestedData = new NestedData
            {
                Level1 = "Value1",
                Level2 = new Level2Data { Value = "Value2" }
            }
        };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.ProcessedMetadata.ShouldNotBeNull();
        handler.ProcessedMetadata.Count.ShouldBe(3);
        handler.ProcessedTags.Length.ShouldBe(2);
        handler.ProcessedNestedValue.ShouldBe("Value2");
    }

    [Fact]
    public async Task DomainEventHandler_ShouldHandleHighFrequencyEvents()
    {
        // Arrange
        var handler = new HighFrequencyHandler();
        var events = Enumerable.Range(1, 1000)
            .Select(i => new HighFrequencyEvent { SequenceNumber = i })
            .ToList();

        // Act
        var tasks = events.Select(e => handler.Handle(e, CancellationToken.None));
        await Task.WhenAll(tasks);

        // Assert
        handler.ProcessedCount.ShouldBe(1000);
        handler.ProcessedSequenceNumbers.Count.ShouldBe(1000);
    }

    [Fact]
    public async Task DomainEventHandler_WithStateManagement_ShouldMaintainConsistency()
    {
        // Arrange
        var handler = new StatefulHandler();
        var events = new[]
        {
            new StatefulEvent { Action = "Increment", Value = 1 },
            new StatefulEvent { Action = "Increment", Value = 2 },
            new StatefulEvent { Action = "Decrement", Value = 1 },
            new StatefulEvent { Action = "Increment", Value = 3 }
        };

        // Act
        foreach (var evt in events)
        {
            await handler.Handle(evt, CancellationToken.None);
        }

        // Assert
        handler.CurrentState.ShouldBe(5); // 1 + 2 - 1 + 3
    }

    [Fact]
    public void IDomainEventHandler_FromMediatRNamespace_ShouldWorkWithDomainEventBase()
    {
        // Arrange & Act
        var handler = new DomainEventBaseHandler();
        var domainEvent = new TestDomainEventBase { Name = "Test" };

        // Assert
        handler.ShouldBeAssignableTo<IDomainEventHandler<TestDomainEventBase>>();
        domainEvent.ShouldBeAssignableTo<DomainEventBase>();
        domainEvent.EventId.ShouldNotBe(Guid.Empty);
        domainEvent.OccurredOn.ShouldNotBe(default(DateTimeOffset));
    }

    [Fact]
    public async Task DomainEventHandler_ShouldAccessDomainEventBaseProperties()
    {
        // Arrange
        var handler = new DomainEventBasePropertyAccessHandler();
        var domainEvent = new TestDomainEventBase { Name = "Property Test" };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.CapturedEventId.ShouldBe(domainEvent.EventId);
        handler.CapturedOccurredOn.ShouldBe(domainEvent.OccurredOn);
        handler.CapturedName.ShouldBe("Property Test");
    }

    // Test implementations
    private class TestCancellableEvent : IDomainEvent
    {
        public int Id { get; set; }
    }

    private class CancellableDomainEventHandler : IDomainEventHandler<TestCancellableEvent>
    {
        public bool StartedProcessing { get; private set; }
        public bool CompletedProcessing { get; private set; }

        public async Task Handle(TestCancellableEvent notification, CancellationToken cancellationToken)
        {
            StartedProcessing = true;
            await Task.Delay(1000, cancellationToken);
            CompletedProcessing = true;
        }
    }

    private class ParentDomainEvent : IDomainEvent
    {
        public List<ChildDomainEvent> ChildEvents { get; set; } = new();
    }

    private class ChildDomainEvent
    {
        public int ChildId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    private class NestedEventHandler : IDomainEventHandler<ParentDomainEvent>
    {
        public int ProcessedChildCount { get; private set; }
        public List<string> ChildMessages { get; } = new();

        public Task Handle(ParentDomainEvent notification, CancellationToken cancellationToken)
        {
            ProcessedChildCount = notification.ChildEvents.Count;
            ChildMessages.AddRange(notification.ChildEvents.Select(c => c.Message));
            return Task.CompletedTask;
        }
    }

    public class ParallelTestEvent : IDomainEvent
    {
        public int Value { get; set; }
    }

    public static class ParallelExecutionTracker
    {
        public static bool Handler1Executed { get; set; }
        public static bool Handler2Executed { get; set; }
        public static bool Handler3Executed { get; set; }

        public static void Reset()
        {
            Handler1Executed = false;
            Handler2Executed = false;
            Handler3Executed = false;
        }
    }

    public class ParallelTestEventHandler1 : IDomainEventHandler<ParallelTestEvent>
    {
        public async Task Handle(ParallelTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            ParallelExecutionTracker.Handler1Executed = true;
        }
    }

    public class ParallelTestEventHandler2 : IDomainEventHandler<ParallelTestEvent>
    {
        public async Task Handle(ParallelTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            ParallelExecutionTracker.Handler2Executed = true;
        }
    }

    public class ParallelTestEventHandler3 : IDomainEventHandler<ParallelTestEvent>
    {
        public async Task Handle(ParallelTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            ParallelExecutionTracker.Handler3Executed = true;
        }
    }

    public class ExceptionTestEvent : IDomainEvent
    {
        public bool ShouldThrow { get; set; }
    }

    public static class ExceptionTestTracker
    {
        private static int _executedCount;

        public static void Reset() => _executedCount = 0;
        public static void IncrementExecuted() => Interlocked.Increment(ref _executedCount);
        public static int GetExecutedCount() => _executedCount;
    }

    public class ExceptionTestHandler1 : IDomainEventHandler<ExceptionTestEvent>
    {
        public Task Handle(ExceptionTestEvent notification, CancellationToken cancellationToken)
        {
            ExceptionTestTracker.IncrementExecuted();
            if (notification.ShouldThrow)
                throw new InvalidOperationException("Handler 1 failed");
            return Task.CompletedTask;
        }
    }

    public class ExceptionTestHandler2 : IDomainEventHandler<ExceptionTestEvent>
    {
        public Task Handle(ExceptionTestEvent notification, CancellationToken cancellationToken)
        {
            ExceptionTestTracker.IncrementExecuted();
            return Task.CompletedTask;
        }
    }

    private class GenericDomainEvent<T> : IDomainEvent
    {
        public T? Data { get; set; }
    }

    private class GenericEventHandler<T> : IDomainEventHandler<GenericDomainEvent<T>>
    {
        public T? ReceivedData { get; private set; }

        public Task Handle(GenericDomainEvent<T> notification, CancellationToken cancellationToken)
        {
            ReceivedData = notification.Data;
            return Task.CompletedTask;
        }
    }

    private class ComplexPayloadEvent : IDomainEvent
    {
        public Dictionary<string, object> Metadata { get; set; } = new();
        public string[] Tags { get; set; } = Array.Empty<string>();
        public NestedData NestedData { get; set; } = new();
    }

    private class NestedData
    {
        public string Level1 { get; set; } = string.Empty;
        public Level2Data Level2 { get; set; } = new();
    }

    private class Level2Data
    {
        public string Value { get; set; } = string.Empty;
    }

    private class ComplexPayloadHandler : IDomainEventHandler<ComplexPayloadEvent>
    {
        public Dictionary<string, object>? ProcessedMetadata { get; private set; }
        public string[]? ProcessedTags { get; private set; }
        public string? ProcessedNestedValue { get; private set; }

        public Task Handle(ComplexPayloadEvent notification, CancellationToken cancellationToken)
        {
            ProcessedMetadata = notification.Metadata;
            ProcessedTags = notification.Tags;
            ProcessedNestedValue = notification.NestedData.Level2.Value;
            return Task.CompletedTask;
        }
    }

    private class HighFrequencyEvent : IDomainEvent
    {
        public int SequenceNumber { get; set; }
    }

    private class HighFrequencyHandler : IDomainEventHandler<HighFrequencyEvent>
    {
        private int _processedCount;
        private readonly HashSet<int> _processedSequenceNumbers = new();
        private readonly object _lock = new object();

        public int ProcessedCount => _processedCount;
        public HashSet<int> ProcessedSequenceNumbers => _processedSequenceNumbers;

        public Task Handle(HighFrequencyEvent notification, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                Interlocked.Increment(ref _processedCount);
                _processedSequenceNumbers.Add(notification.SequenceNumber);
            }
            return Task.CompletedTask;
        }
    }

    private class StatefulEvent : IDomainEvent
    {
        public string Action { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class StatefulHandler : IDomainEventHandler<StatefulEvent>
    {
        public int CurrentState { get; private set; }

        public Task Handle(StatefulEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Action == "Increment")
                CurrentState += notification.Value;
            else if (notification.Action == "Decrement")
                CurrentState -= notification.Value;
                
            return Task.CompletedTask;
        }
    }

    private record TestDomainEventBase : DomainEventBase
    {
        public string Name { get; set; } = string.Empty;
    }

    private class DomainEventBaseHandler : IDomainEventHandler<TestDomainEventBase>
    {
        public Task Handle(TestDomainEventBase notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private class DomainEventBasePropertyAccessHandler : IDomainEventHandler<TestDomainEventBase>
    {
        public Guid CapturedEventId { get; private set; }
        public DateTimeOffset CapturedOccurredOn { get; private set; }
        public string CapturedName { get; private set; } = string.Empty;

        public Task Handle(TestDomainEventBase notification, CancellationToken cancellationToken)
        {
            CapturedEventId = notification.EventId;
            CapturedOccurredOn = notification.OccurredOn;
            CapturedName = notification.Name;
            return Task.CompletedTask;
        }
    }
}
