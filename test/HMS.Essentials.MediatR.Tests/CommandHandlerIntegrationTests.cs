using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.Tests;

public class CommandHandlerIntegrationTests
{
    [Fact]
    public void ICommandHandler_ShouldImplementMediatRInterface()
    {
        // Assert
        typeof(IRequestHandler<TestCommand, Unit>).IsAssignableFrom(typeof(ICommandHandler<TestCommand>)).ShouldBeTrue();
    }

    [Fact]
    public void ICommandHandlerWithResponse_ShouldImplementMediatRInterface()
    {
        // Assert
        typeof(IRequestHandler<TestCommandWithResponse, string>)
            .IsAssignableFrom(typeof(ICommandHandler<TestCommandWithResponse, string>)).ShouldBeTrue();
    }

    [Fact]
    public async Task CommandHandler_ShouldExecuteCorrectly()
    {
        // Arrange
        var handler = new TestCommandHandler();
        var command = new TestCommand { Value = "test" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        handler.WasExecuted.ShouldBeTrue();
    }

    [Fact]
    public async Task CommandHandlerWithResponse_ShouldReturnCorrectValue()
    {
        // Arrange
        var handler = new TestCommandWithResponseHandler();
        var command = new TestCommandWithResponse { Input = "Hello" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe("Hello World");
    }

    private class TestCommand : ICommand
    {
        public string Value { get; set; } = string.Empty;
    }

    private class TestCommandWithResponse : ICommand<string>
    {
        public string Input { get; set; } = string.Empty;
    }

    private class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public bool WasExecuted { get; private set; }

        public Task<Unit> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            WasExecuted = true;
            return Task.FromResult(Unit.Value);
        }
    }

    private class TestCommandWithResponseHandler : ICommandHandler<TestCommandWithResponse, string>
    {
        public Task<string> Handle(TestCommandWithResponse request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"{request.Input} World");
        }
    }
}
