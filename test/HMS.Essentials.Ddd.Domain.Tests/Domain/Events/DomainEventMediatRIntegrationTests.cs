using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Domain.Events;

public class DomainEventMediatRIntegrationTests
{
    [Fact]
    public async Task DomainEvent_PublishedThroughMediatR_ShouldBeHandled()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventMediatRIntegrationTests).Assembly));
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        var domainEvent = new PublishableDomainEvent { Message = "Test" };

        // Act
        await mediator.Publish(domainEvent);

        // Assert
        PublishableDomainEventHandler.LastHandledEvent.ShouldNotBeNull();
        PublishableDomainEventHandler.LastHandledEvent.Message.ShouldBe("Test");
    }

    [Fact]
    public async Task MultipleDomainEventHandlers_ShouldAllReceiveEvent()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventMediatRIntegrationTests).Assembly));
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        var domainEvent = new MultiHandlerEvent { Value = 42 };
        
        // Reset static handlers
        MultiHandlerEvent1.HandledCount = 0;
        MultiHandlerEvent2.HandledCount = 0;

        // Act
        await mediator.Publish(domainEvent);

        // Assert
        MultiHandlerEvent1.HandledCount.ShouldBe(1);
        MultiHandlerEvent2.HandledCount.ShouldBe(1);
    }

    [Fact]
    public async Task DomainEvent_ShouldWorkAsINotification()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventMediatRIntegrationTests).Assembly));
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        INotification notification = new PublishableDomainEvent { Message = "As Notification" };
        PublishableDomainEventHandler.LastHandledEvent = null;

        // Act
        await mediator.Publish(notification);

        // Assert
        PublishableDomainEventHandler.LastHandledEvent.ShouldNotBeNull();
        PublishableDomainEventHandler.LastHandledEvent.Message.ShouldBe("As Notification");
    }

    [Fact]
    public async Task DomainEventHandler_CanAccessDependencyInjectedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ITestService, TestService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventMediatRIntegrationTests).Assembly));
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        var domainEvent = new DependencyEvent { Data = "Test Data" };
        DependencyEventHandler.LastServiceMessage = null;

        // Act
        await mediator.Publish(domainEvent);

        // Assert
        DependencyEventHandler.LastServiceMessage.ShouldBe("Processed: Test Data");
    }

    [Fact]
    public async Task DomainEvent_WithNoHandlers_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventMediatRIntegrationTests).Assembly));
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        var domainEvent = new EventWithNoHandlers();

        // Act & Assert
        await Should.NotThrowAsync(async () => await mediator.Publish(domainEvent));
    }

    // Test implementations
    public class PublishableDomainEvent : DomainEvent
    {
        public string Message { get; set; } = string.Empty;
    }

    public class PublishableDomainEventHandler : IDomainEventHandler<PublishableDomainEvent>
    {
        public static PublishableDomainEvent? LastHandledEvent { get; set; }

        public Task Handle(PublishableDomainEvent notification, CancellationToken cancellationToken)
        {
            LastHandledEvent = notification;
            return Task.CompletedTask;
        }
    }

    public class MultiHandlerEvent : DomainEvent
    {
        public int Value { get; set; }
    }

    public class MultiHandlerEvent1 : IDomainEventHandler<MultiHandlerEvent>
    {
        public static int HandledCount { get; set; }

        public Task Handle(MultiHandlerEvent notification, CancellationToken cancellationToken)
        {
            HandledCount++;
            return Task.CompletedTask;
        }
    }

    public class MultiHandlerEvent2 : IDomainEventHandler<MultiHandlerEvent>
    {
        public static int HandledCount { get; set; }

        public Task Handle(MultiHandlerEvent notification, CancellationToken cancellationToken)
        {
            HandledCount++;
            return Task.CompletedTask;
        }
    }

    public interface ITestService
    {
        string Process(string input);
    }

    public class TestService : ITestService
    {
        public string Process(string input) => $"Processed: {input}";
    }

    public class DependencyEvent : DomainEvent
    {
        public string Data { get; set; } = string.Empty;
    }

    public class DependencyEventHandler : IDomainEventHandler<DependencyEvent>
    {
        public static string? LastServiceMessage { get; set; }
        private readonly ITestService _testService;

        public DependencyEventHandler(ITestService testService)
        {
            _testService = testService;
        }

        public Task Handle(DependencyEvent notification, CancellationToken cancellationToken)
        {
            LastServiceMessage = _testService.Process(notification.Data);
            return Task.CompletedTask;
        }
    }

    public class EventWithNoHandlers : DomainEvent
    {
    }
}
