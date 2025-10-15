using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.AspNetCore.TestModules;

/// <summary>
/// Test module without dependencies for testing purposes.
/// </summary>
public class TestModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        base.ConfigureServices(context);
        context.Services.AddSingleton<ITestService, TestService>();
    }
}

/// <summary>
/// Another test module with dependency on TestModule.
/// </summary>
[DependsOn(typeof(TestModule))]
public class DependentTestModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        base.ConfigureServices(context);
        context.Services.AddSingleton<IDependentService, DependentService>();
    }
}

/// <summary>
/// Root test module for integration tests.
/// </summary>
[DependsOn(typeof(DependentTestModule))]
public class RootTestModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        base.ConfigureServices(context);
        context.Services.AddSingleton<IRootService, RootService>();
    }
}

// Test services
public interface ITestService
{
    string GetMessage();
}

public class TestService : ITestService
{
    public string GetMessage() => "Test Service";
}

public interface IDependentService
{
    string GetMessage();
}

public class DependentService : IDependentService
{
    private readonly ITestService _testService;

    public DependentService(ITestService testService)
    {
        _testService = testService;
    }

    public string GetMessage() => $"Dependent Service using {_testService.GetMessage()}";
}

public interface IRootService
{
    string GetMessage();
}

public class RootService : IRootService
{
    private readonly IDependentService _dependentService;

    public RootService(IDependentService dependentService)
    {
        _dependentService = dependentService;
    }

    public string GetMessage() => $"Root Service using {_dependentService.GetMessage()}";
}
