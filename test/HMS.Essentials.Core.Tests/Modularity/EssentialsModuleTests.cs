using Shouldly;

namespace HMS.Essentials.Modularity;

public class EssentialsModuleTests
{
    [Fact]
    public void Module_Should_Be_Instantiable()
    {
        // Act
        var module = new TestModule();

        // Assert
        module.ShouldNotBeNull();
        module.ShouldBeAssignableTo<IModule>();
    }

    [Fact]
    public void ConfigureServices_Should_Be_Virtual_And_Overridable()
    {
        // Arrange
        var module = new ModuleWithConfigureServices();
        var context = TestHelper.CreateModuleContext();

        // Act
        module.ConfigureServices(context);

        // Assert
        module.ConfigureServicesCalled.ShouldBeTrue();
    }

    [Fact]
    public void Initialize_Should_Be_Virtual_And_Overridable()
    {
        // Arrange
        var module = new ModuleWithInitialize();
        var context = TestHelper.CreateModuleContext();

        // Act
        module.Initialize(context);

        // Assert
        module.InitializeCalled.ShouldBeTrue();
    }

    [Fact]
    public void Shutdown_Should_Be_Virtual_And_Overridable()
    {
        // Arrange
        var module = new ModuleWithShutdown();
        var context = TestHelper.CreateModuleContext();

        // Act
        module.Shutdown(context);

        // Assert
        module.ShutdownCalled.ShouldBeTrue();
    }

    [Fact]
    public void Default_ConfigureServices_Should_Do_Nothing()
    {
        // Arrange
        var module = new TestModule();
        var context = TestHelper.CreateModuleContext();
        var serviceCountBefore = context.Services.Count;

        // Act
        module.ConfigureServices(context);

        // Assert
        context.Services.Count.ShouldBe(serviceCountBefore);
    }

    [Fact]
    public void Default_Lifecycle_Methods_Should_Do_Nothing()
    {
        // Arrange
        var module = new TestModule();
        var context = TestHelper.CreateModuleContext();

        // Act & Assert - Should not throw
        Should.NotThrow(() => module.Initialize(context));
        Should.NotThrow(() => module.Shutdown(context));
    }

    private class TestModule : EssentialsModule
    {
    }

    private class ModuleWithConfigureServices : EssentialsModule
    {
        public bool ConfigureServicesCalled { get; private set; }

        public override void ConfigureServices(ModuleContext context)
        {
            ConfigureServicesCalled = true;
            base.ConfigureServices(context);
        }
    }

    private class ModuleWithInitialize : EssentialsModule
    {
        public bool InitializeCalled { get; private set; }

        public override void Initialize(ModuleContext context)
        {
            InitializeCalled = true;
            base.Initialize(context);
        }
    }

    private class ModuleWithShutdown : EssentialsModule
    {
        public bool ShutdownCalled { get; private set; }

        public override void Shutdown(ModuleContext context)
        {
            ShutdownCalled = true;
            base.Shutdown(context);
        }
    }
}
