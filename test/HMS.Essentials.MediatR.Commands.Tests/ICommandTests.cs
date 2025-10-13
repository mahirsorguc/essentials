using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.Commands.Tests;

public class ICommandTests
{
    [Fact]
    public void ICommand_ShouldImplementMediatRRequest()
    {
        // Assert
        typeof(IRequest<Unit>).IsAssignableFrom(typeof(ICommand)).ShouldBeTrue();
    }

    [Fact]
    public void ICommandWithResponse_ShouldImplementMediatRRequest()
    {
        // Assert
        typeof(IRequest<string>).IsAssignableFrom(typeof(ICommand<string>)).ShouldBeTrue();
    }

    [Fact]
    public void ConcreteCommand_ShouldImplementICommand()
    {
        // Arrange & Act
        var command = new TestCommand();

        // Assert
        command.ShouldBeAssignableTo<ICommand>();
        command.ShouldBeAssignableTo<IRequest<Unit>>();
    }

    [Fact]
    public void ConcreteCommandWithResponse_ShouldImplementICommand()
    {
        // Arrange & Act
        var command = new TestCommandWithResponse();

        // Assert
        command.ShouldBeAssignableTo<ICommand<string>>();
        command.ShouldBeAssignableTo<IRequest<string>>();
    }

    [Fact]
    public void ICommand_ShouldBeMarkerInterface()
    {
        // Assert - ICommand should have no methods or properties
        var methods = typeof(ICommand).GetMethods();
        var properties = typeof(ICommand).GetProperties();

        // Only inherited methods from IRequest should exist
        methods.Length.ShouldBe(0);
        properties.Length.ShouldBe(0);
    }

    [Fact]
    public void ICommandWithResponse_ShouldBeMarkerInterface()
    {
        // Assert - ICommand<T> should have no methods or properties
        var methods = typeof(ICommand<>).GetMethods();
        var properties = typeof(ICommand<>).GetProperties();

        // Only inherited methods from IRequest should exist
        methods.Length.ShouldBe(0);
        properties.Length.ShouldBe(0);
    }

    [Fact]
    public void MultipleCommands_ShouldBeDistinct()
    {
        // Arrange & Act
        var command1 = new TestCommand();
        var command2 = new TestCommand();

        // Assert
        command1.ShouldNotBeSameAs(command2);
    }

    private class TestCommand : ICommand
    {
    }

    private class TestCommandWithResponse : ICommand<string>
    {
    }
}
