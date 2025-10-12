using Shouldly;

namespace HMS.Essentials.Modularity;

public class ModuleStateTests
{
    [Fact]
    public void ModuleState_Should_Have_All_Expected_Values()
    {
        // Assert
        Enum.IsDefined(typeof(ModuleState), ModuleState.Discovered).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.Loading).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.Loaded).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.ConfiguringServices).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.ServicesConfigured).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.Initializing).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.Initialized).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.ShuttingDown).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.ShutDown).ShouldBeTrue();
        Enum.IsDefined(typeof(ModuleState), ModuleState.Error).ShouldBeTrue();
    }

    [Fact]
    public void ModuleState_Values_Should_Have_Correct_Numeric_Values()
    {
        // Assert
        ((int)ModuleState.Discovered).ShouldBe(0);
        ((int)ModuleState.Loading).ShouldBe(1);
        ((int)ModuleState.Loaded).ShouldBe(2);
        ((int)ModuleState.ConfiguringServices).ShouldBe(3);
        ((int)ModuleState.ServicesConfigured).ShouldBe(4);
        ((int)ModuleState.Initializing).ShouldBe(5);
        ((int)ModuleState.Initialized).ShouldBe(6);
        ((int)ModuleState.ShuttingDown).ShouldBe(7);
        ((int)ModuleState.ShutDown).ShouldBe(8);
        ((int)ModuleState.Error).ShouldBe(9);
    }

    [Theory]
    [InlineData(ModuleState.Discovered)]
    [InlineData(ModuleState.Loading)]
    [InlineData(ModuleState.Loaded)]
    [InlineData(ModuleState.ConfiguringServices)]
    [InlineData(ModuleState.ServicesConfigured)]
    [InlineData(ModuleState.Initializing)]
    [InlineData(ModuleState.Initialized)]
    [InlineData(ModuleState.ShuttingDown)]
    [InlineData(ModuleState.ShutDown)]
    [InlineData(ModuleState.Error)]
    public void ModuleState_Values_Should_Be_Convertible_To_String(ModuleState state)
    {
        // Act
        var stateString = state.ToString();

        // Assert
        stateString.ShouldNotBeNullOrEmpty();
    }
}
