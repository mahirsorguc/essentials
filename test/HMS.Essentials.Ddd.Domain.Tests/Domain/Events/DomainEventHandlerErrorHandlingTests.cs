namespace HMS.Essentials.Domain.Events;

public class DomainEventHandlerErrorHandlingTests
{
    [Fact]
    public async Task DomainEventHandler_ShouldRespectCancellationToken()
    {
        // Arrange
        var handler = new CancellableHandler();
        var domainEvent = new TestEvent { Value = 10 };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await handler.Handle(domainEvent, cts.Token);
        });
    }

    [Fact]
    public async Task DomainEventHandler_ShouldPropagateCancellationDuringExecution()
    {
        // Arrange
        var handler = new DelayedHandler();
        var domainEvent = new TestEvent { Value = 20 };
        var cts = new CancellationTokenSource();
        cts.CancelAfter(50); // Cancel after 50ms

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await handler.Handle(domainEvent, cts.Token);
        });
    }

    [Fact]
    public async Task DomainEventHandler_ThrowingException_ShouldPropagateException()
    {
        // Arrange
        var handler = new ThrowingHandler();
        var domainEvent = new TestEvent { Value = 30 };

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await handler.Handle(domainEvent, CancellationToken.None);
        });
        
        exception.Message.ShouldBe("Handler error occurred");
    }

    [Fact]
    public async Task DomainEventHandler_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        var handler = new StrictHandler();

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await handler.Handle(null!, CancellationToken.None);
        });
    }

    [Fact]
    public async Task DomainEventHandler_WithAsyncException_ShouldPropagateCorrectly()
    {
        // Arrange
        var handler = new AsyncExceptionHandler();
        var domainEvent = new TestEvent { Value = 40 };

        // Act & Assert
        var exception = await Should.ThrowAsync<ApplicationException>(async () =>
        {
            await handler.Handle(domainEvent, CancellationToken.None);
        });
        
        exception.Message.ShouldBe("Async operation failed");
    }

    [Fact]
    public async Task DomainEventHandler_ShouldHandleTimeoutScenarios()
    {
        // Arrange
        var handler = new TimeoutHandler();
        var domainEvent = new TestEvent { Value = 50 };
        var cts = new CancellationTokenSource(100); // 100ms timeout

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await handler.Handle(domainEvent, cts.Token);
        });
    }

    [Fact]
    public async Task DomainEventHandler_WithValidationError_ShouldThrowException()
    {
        // Arrange
        var handler = new ValidatingHandler();
        var invalidEvent = new TestEvent { Value = -1 }; // Invalid value

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await handler.Handle(invalidEvent, CancellationToken.None);
        });
        
        exception.Message.ShouldContain("Value must be positive");
    }

    [Fact]
    public async Task DomainEventHandler_WithMultipleErrors_ShouldThrowFirstError()
    {
        // Arrange
        var handler = new MultipleErrorHandler();
        var domainEvent = new TestEvent { Value = 60 };

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await handler.Handle(domainEvent, CancellationToken.None);
        });
        
        exception.Message.ShouldBe("First error");
    }

    [Fact]
    public async Task DomainEventHandler_RecoveringFromError_ShouldComplete()
    {
        // Arrange
        var handler = new RecoverableHandler();
        var domainEvent = new TestEvent { Value = 70 };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.WasRecovered.ShouldBeTrue();
        handler.FinalValue.ShouldBe(70);
    }

    [Fact]
    public async Task DomainEventHandler_WithRetryLogic_ShouldEventuallySucceed()
    {
        // Arrange
        var handler = new RetryHandler();
        var domainEvent = new TestEvent { Value = 80 };

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        handler.AttemptCount.ShouldBeGreaterThan(1);
        handler.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task DomainEventHandler_CancellationAfterCompletion_ShouldNotAffectResult()
    {
        // Arrange
        var handler = new FastHandler();
        var domainEvent = new TestEvent { Value = 90 };
        var cts = new CancellationTokenSource();

        // Act
        await handler.Handle(domainEvent, cts.Token);
        cts.Cancel(); // Cancel after completion

        // Assert
        handler.Completed.ShouldBeTrue();
        handler.HandledValue.ShouldBe(90);
    }

    // Test implementations
    private class TestEvent : DomainEvent
    {
        public int Value { get; set; }
    }

    private class CancellableHandler : DomainEventHandler<TestEvent>
    {
        public override Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
    }

    private class DelayedHandler : DomainEventHandler<TestEvent>
    {
        public override async Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken); // Long delay to allow cancellation
        }
    }

    private class ThrowingHandler : DomainEventHandler<TestEvent>
    {
        public override Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Handler error occurred");
        }
    }

    private class StrictHandler : DomainEventHandler<TestEvent>
    {
        public override Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
                
            return Task.CompletedTask;
        }
    }

    private class AsyncExceptionHandler : DomainEventHandler<TestEvent>
    {
        public override async Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            throw new ApplicationException("Async operation failed");
        }
    }

    private class TimeoutHandler : DomainEventHandler<TestEvent>
    {
        public override async Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            // Simulate long-running operation
            await Task.Delay(10000, cancellationToken);
        }
    }

    private class ValidatingHandler : DomainEventHandler<TestEvent>
    {
        public override Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Value < 0)
                throw new ArgumentException("Value must be positive");
                
            return Task.CompletedTask;
        }
    }

    private class MultipleErrorHandler : DomainEventHandler<TestEvent>
    {
        public override Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("First error");
            // This would never be reached
            #pragma warning disable CS0162
            throw new InvalidOperationException("Second error");
            #pragma warning restore CS0162
        }
    }

    private class RecoverableHandler : DomainEventHandler<TestEvent>
    {
        public bool WasRecovered { get; private set; }
        public int FinalValue { get; private set; }

        public override async Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(10, cancellationToken);
                throw new InvalidOperationException("Temporary error");
            }
            catch (InvalidOperationException)
            {
                WasRecovered = true;
                FinalValue = notification.Value;
            }
        }
    }

    private class RetryHandler : DomainEventHandler<TestEvent>
    {
        public int AttemptCount { get; private set; }
        public bool Succeeded { get; private set; }

        public override async Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            const int maxAttempts = 3;
            
            while (AttemptCount < maxAttempts)
            {
                AttemptCount++;
                await Task.Delay(10, cancellationToken);
                
                if (AttemptCount >= 2) // Succeed on 2nd attempt
                {
                    Succeeded = true;
                    return;
                }
            }
        }
    }

    private class FastHandler : DomainEventHandler<TestEvent>
    {
        public bool Completed { get; private set; }
        public int HandledValue { get; private set; }

        public override Task Handle(TestEvent notification, CancellationToken cancellationToken)
        {
            HandledValue = notification.Value;
            Completed = true;
            return Task.CompletedTask;
        }
    }
}
