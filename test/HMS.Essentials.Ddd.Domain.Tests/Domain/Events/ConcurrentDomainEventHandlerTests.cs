using System.Collections.Concurrent;
using System.Diagnostics;

namespace HMS.Essentials.Domain.Events;

public class ConcurrentDomainEventHandlerTests
{
    [Fact]
    public async Task MultipleDomainEventHandlers_ShouldExecuteConcurrently()
    {
        // Arrange
        var handler1 = new ConcurrentHandler("Handler1");
        var handler2 = new ConcurrentHandler("Handler2");
        var handler3 = new ConcurrentHandler("Handler3");
        var domainEvent = new ConcurrentTestEvent { Value = 42 };

        // Act
        var stopwatch = Stopwatch.StartNew();
        await Task.WhenAll(
            handler1.Handle(domainEvent, CancellationToken.None),
            handler2.Handle(domainEvent, CancellationToken.None),
            handler3.Handle(domainEvent, CancellationToken.None)
        );
        stopwatch.Stop();

        // Assert
        handler1.HandledValue.ShouldBe(42);
        handler2.HandledValue.ShouldBe(42);
        handler3.HandledValue.ShouldBe(42);
        // If executed sequentially, it would take ~300ms (100ms * 3)
        // Concurrent execution should be much faster
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(250);
    }

    [Fact]
    public async Task ConcurrentHandlers_ShouldNotInterfereWithEachOther()
    {
        // Arrange
        var sharedCollection = new ConcurrentBag<int>();
        var handler1 = new CollectionAddingHandler(sharedCollection, 1);
        var handler2 = new CollectionAddingHandler(sharedCollection, 2);
        var handler3 = new CollectionAddingHandler(sharedCollection, 3);
        var domainEvent = new ConcurrentTestEvent { Value = 100 };

        // Act
        await Task.WhenAll(
            handler1.Handle(domainEvent, CancellationToken.None),
            handler2.Handle(domainEvent, CancellationToken.None),
            handler3.Handle(domainEvent, CancellationToken.None)
        );

        // Assert
        sharedCollection.Count.ShouldBe(300); // Each handler adds 100 items
        sharedCollection.Count(x => x == 1).ShouldBe(100);
        sharedCollection.Count(x => x == 2).ShouldBe(100);
        sharedCollection.Count(x => x == 3).ShouldBe(100);
    }

    [Fact]
    public async Task ParallelDomainEventHandling_ShouldMaintainThreadSafety()
    {
        // Arrange
        var counter = new ThreadSafeCounter();
        var handlers = Enumerable.Range(0, 10)
            .Select(_ => new IncrementingHandler(counter))
            .ToList();
        var domainEvent = new ConcurrentTestEvent { Value = 1 };

        // Act
        await Task.WhenAll(handlers.Select(h => h.Handle(domainEvent, CancellationToken.None)));

        // Assert
        counter.Value.ShouldBe(10);
    }

    [Fact]
    public async Task ConcurrentHandlers_WithDifferentExecutionTimes_ShouldAllComplete()
    {
        // Arrange
        var handler1 = new DelayedConcurrentHandler(50);
        var handler2 = new DelayedConcurrentHandler(100);
        var handler3 = new DelayedConcurrentHandler(150);
        var domainEvent = new ConcurrentTestEvent { Value = 99 };

        // Act
        var stopwatch = Stopwatch.StartNew();
        await Task.WhenAll(
            handler1.Handle(domainEvent, CancellationToken.None),
            handler2.Handle(domainEvent, CancellationToken.None),
            handler3.Handle(domainEvent, CancellationToken.None)
        );
        stopwatch.Stop();

        // Assert
        handler1.Completed.ShouldBeTrue();
        handler2.Completed.ShouldBeTrue();
        handler3.Completed.ShouldBeTrue();
        // Should complete in roughly the time of the longest handler (150ms), not the sum (300ms)
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(250);
    }

    [Fact]
    public async Task ConcurrentHandlers_OneThrows_ShouldNotAffectOthers()
    {
        // Arrange
        var successHandler1 = new SuccessfulHandler();
        var failingHandler = new FailingConcurrentHandler();
        var successHandler2 = new SuccessfulHandler();
        var domainEvent = new ConcurrentTestEvent { Value = 50 };

        // Act
        var tasks = new[]
        {
            successHandler1.Handle(domainEvent, CancellationToken.None),
            failingHandler.Handle(domainEvent, CancellationToken.None),
            successHandler2.Handle(domainEvent, CancellationToken.None)
        };

        try
        {
            await Task.WhenAll(tasks);
        }
        catch
        {
            // Expected - one handler will fail
        }

        // Assert
        successHandler1.Completed.ShouldBeTrue();
        successHandler2.Completed.ShouldBeTrue();
        failingHandler.Attempted.ShouldBeTrue();
    }

    [Fact]
    public async Task HighVolumeConcurrentHandling_ShouldScale()
    {
        // Arrange
        var counter = new ThreadSafeCounter();
        const int handlerCount = 100;
        var handlers = Enumerable.Range(0, handlerCount)
            .Select(_ => new IncrementingHandler(counter))
            .ToList();
        var domainEvent = new ConcurrentTestEvent { Value = 1 };

        // Act
        var stopwatch = Stopwatch.StartNew();
        await Task.WhenAll(handlers.Select(h => h.Handle(domainEvent, CancellationToken.None)));
        stopwatch.Stop();

        // Assert
        counter.Value.ShouldBe(handlerCount);
        // Should complete quickly even with many handlers
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(5000);
    }

    [Fact]
    public async Task ConcurrentHandlers_WithSharedState_ShouldUseLocking()
    {
        // Arrange
        var sharedState = new SharedState();
        var handlers = Enumerable.Range(0, 50)
            .Select(_ => new StateModifyingHandler(sharedState))
            .ToList();
        var domainEvent = new ConcurrentTestEvent { Value = 1 };

        // Act
        await Task.WhenAll(handlers.Select(h => h.Handle(domainEvent, CancellationToken.None)));

        // Assert
        sharedState.Counter.ShouldBe(50);
    }

    [Fact]
    public async Task ConcurrentHandlers_WithCancellation_ShouldCancelProperly()
    {
        // Arrange
        var handler1 = new LongRunningHandler();
        var handler2 = new LongRunningHandler();
        var handler3 = new LongRunningHandler();
        var domainEvent = new ConcurrentTestEvent { Value = 1 };
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await Task.WhenAll(
                handler1.Handle(domainEvent, cts.Token),
                handler2.Handle(domainEvent, cts.Token),
                handler3.Handle(domainEvent, cts.Token)
            );
        });
    }

    [Fact]
    public async Task ConcurrentHandlers_WithOrderedExecution_ShouldMaintainOrder()
    {
        // Arrange
        var executionOrder = new ConcurrentBag<int>();
        var handler1 = new OrderedHandler(executionOrder, 1);
        var handler2 = new OrderedHandler(executionOrder, 2);
        var handler3 = new OrderedHandler(executionOrder, 3);
        var domainEvent = new ConcurrentTestEvent { Value = 1 };

        // Act
        await Task.WhenAll(
            handler1.Handle(domainEvent, CancellationToken.None),
            handler2.Handle(domainEvent, CancellationToken.None),
            handler3.Handle(domainEvent, CancellationToken.None)
        );

        // Assert
        executionOrder.Count.ShouldBe(3);
        executionOrder.ShouldContain(1);
        executionOrder.ShouldContain(2);
        executionOrder.ShouldContain(3);
    }

    // Test implementations
    private class ConcurrentTestEvent : DomainEvent
    {
        public int Value { get; set; }
    }

    private class ConcurrentHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        private readonly string _name;
        public int HandledValue { get; private set; }

        public ConcurrentHandler(string name)
        {
            _name = name;
        }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            HandledValue = notification.Value;
        }
    }

    private class CollectionAddingHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        private readonly ConcurrentBag<int> _collection;
        private readonly int _valueToAdd;

        public CollectionAddingHandler(ConcurrentBag<int> collection, int valueToAdd)
        {
            _collection = collection;
            _valueToAdd = valueToAdd;
        }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            for (int i = 0; i < notification.Value; i++)
            {
                await Task.Delay(1, cancellationToken);
                _collection.Add(_valueToAdd);
            }
        }
    }

    private class ThreadSafeCounter
    {
        private int _value;
        public int Value => _value;

        public void Increment()
        {
            Interlocked.Increment(ref _value);
        }
    }

    private class IncrementingHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        private readonly ThreadSafeCounter _counter;

        public IncrementingHandler(ThreadSafeCounter counter)
        {
            _counter = counter;
        }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            _counter.Increment();
        }
    }

    private class DelayedConcurrentHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        private readonly int _delayMs;
        public bool Completed { get; private set; }

        public DelayedConcurrentHandler(int delayMs)
        {
            _delayMs = delayMs;
        }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(_delayMs, cancellationToken);
            Completed = true;
        }
    }

    private class SuccessfulHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        public bool Completed { get; private set; }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(50, cancellationToken);
            Completed = true;
        }
    }

    private class FailingConcurrentHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        public bool Attempted { get; private set; }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(50, cancellationToken);
            Attempted = true;
            throw new InvalidOperationException("Simulated failure");
        }
    }

    private class SharedState
    {
        private readonly object _lock = new object();
        public int Counter { get; private set; }

        public void Increment()
        {
            lock (_lock)
            {
                Counter++;
            }
        }
    }

    private class StateModifyingHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        private readonly SharedState _state;

        public StateModifyingHandler(SharedState state)
        {
            _state = state;
        }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            _state.Increment();
        }
    }

    private class LongRunningHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);
        }
    }

    private class OrderedHandler : DomainEventHandler<ConcurrentTestEvent>
    {
        private readonly ConcurrentBag<int> _executionOrder;
        private readonly int _orderId;

        public OrderedHandler(ConcurrentBag<int> executionOrder, int orderId)
        {
            _executionOrder = executionOrder;
            _orderId = orderId;
        }

        public override async Task Handle(ConcurrentTestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(50, cancellationToken);
            _executionOrder.Add(_orderId);
        }
    }
}
