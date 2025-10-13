using MediatR;
using Shouldly;

namespace HMS.Essentials.MediatR.Commands.Tests;

public class CommandBaseTests
{
    [Fact]
    public void CommandBase_ShouldImplementICommand()
    {
        // Arrange & Act
        var command = new TestCommandBase();

        // Assert
        command.ShouldBeAssignableTo<CommandBase>();
        command.ShouldBeAssignableTo<ICommand>();
        command.ShouldBeAssignableTo<IRequest<Unit>>();
    }

    [Fact]
    public void CommandBase_ShouldSetCreatedAtOnCreation()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var command = new TestCommandBase();

        // Assert
        var afterCreation = DateTimeOffset.UtcNow;
        command.CreatedAt.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        command.CreatedAt.ShouldBeLessThanOrEqualTo(afterCreation);
    }

    [Fact]
    public void CommandBase_CreatedAt_ShouldBeReadOnly()
    {
        // Arrange & Act
        var command = new TestCommandBase();
        var initialCreatedAt = command.CreatedAt;

        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Assert - CreatedAt should not change
        command.CreatedAt.ShouldBe(initialCreatedAt);
    }

    [Fact]
    public void CommandBase_WithProperties_ShouldWorkAsRecord()
    {
        // Arrange & Act
        var command1 = new TestCommandWithProperties("test", 123);
        var command2 = new TestCommandWithProperties("test", 123);

        // Assert - Records with same values should be equal (except CreatedAt)
        command1.Name.ShouldBe(command2.Name);
        command1.Value.ShouldBe(command2.Value);
    }

    [Fact]
    public void CommandBase_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange & Act
        var command1 = new TestCommandWithProperties("test1", 123);
        var command2 = new TestCommandWithProperties("test2", 456);

        // Assert
        command1.ShouldNotBe(command2);
        command1.Name.ShouldNotBe(command2.Name);
        command1.Value.ShouldNotBe(command2.Value);
    }

    [Fact]
    public void CommandBaseWithResponse_ShouldImplementICommand()
    {
        // Arrange & Act
        var command = new TestCommandBaseWithResponse();

        // Assert
        command.ShouldBeAssignableTo<CommandBase<string>>();
        command.ShouldBeAssignableTo<ICommand<string>>();
        command.ShouldBeAssignableTo<IRequest<string>>();
    }

    [Fact]
    public void CommandBaseWithResponse_ShouldSetCreatedAtOnCreation()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var command = new TestCommandBaseWithResponse();

        // Assert
        var afterCreation = DateTimeOffset.UtcNow;
        command.CreatedAt.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        command.CreatedAt.ShouldBeLessThanOrEqualTo(afterCreation);
    }

    [Fact]
    public void CommandBaseWithResponse_CreatedAt_ShouldBeReadOnly()
    {
        // Arrange & Act
        var command = new TestCommandBaseWithResponse();
        var initialCreatedAt = command.CreatedAt;

        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Assert - CreatedAt should not change
        command.CreatedAt.ShouldBe(initialCreatedAt);
    }

    [Fact]
    public void CommandBaseWithResponse_WithProperties_ShouldWorkAsRecord()
    {
        // Arrange & Act
        var command1 = new TestCommandWithResponseAndProperties(42);
        var command2 = new TestCommandWithResponseAndProperties(42);

        // Assert
        command1.Id.ShouldBe(command2.Id);
    }

    [Fact]
    public void MultipleCommands_ShouldHaveDifferentCreationTimes()
    {
        // Arrange & Act
        var command1 = new TestCommandBase();
        Thread.Sleep(10);
        var command2 = new TestCommandBase();

        // Assert
        command2.CreatedAt.ShouldBeGreaterThan(command1.CreatedAt);
    }

    [Fact]
    public void CommandBase_ShouldBeRecord()
    {
        // Assert - Records have with expressions and deconstruction
        var command = new TestCommandWithProperties("test", 123);
        var modifiedCommand = command with { Name = "modified" };

        modifiedCommand.Name.ShouldBe("modified");
        modifiedCommand.Value.ShouldBe(123);
        command.Name.ShouldBe("test"); // Original should be unchanged
    }

    private record TestCommandBase : CommandBase
    {
    }

    private record TestCommandWithProperties(string Name, int Value) : CommandBase
    {
    }

    private record TestCommandBaseWithResponse : CommandBase<string>
    {
    }

    private record TestCommandWithResponseAndProperties(int Id) : CommandBase<string>
    {
    }
}
