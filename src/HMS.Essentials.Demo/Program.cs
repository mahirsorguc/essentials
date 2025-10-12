using HMS.Essential.Logging;
using HMS.Essentials.Modularity;
using HMS.Essentials.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== HMS Essentials Modularity Demo ===\n");

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

// Create service collection
var services = new ServiceCollection();

// Build the application with modules
var app = new ApplicationBuilder(services, configuration)
    .WithEnvironment("Development")
    .AddModule<LoggingEssentialsModule>()
    .AddModule<EssentialsDataModule>()
    .ConfigureServices((svc, cfg) =>
    {
        Console.WriteLine("[Application] Configuring additional services...");
    })
    .Build();

Console.WriteLine("\n=== Application Built Successfully ===\n");

// Demonstrate module usage
await DemonstrateModularityAsync(app);

Console.WriteLine("\n=== Shutting Down ===\n");

// Cleanup
app.Dispose();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

static async Task DemonstrateModularityAsync(ApplicationHost app)
{
    Console.WriteLine("--- Module Information ---");
    foreach (var module in app.Modules.OrderBy(m => m.Priority).Reverse())
    {
        Console.WriteLine($"Module: {module.Name}");
        Console.WriteLine($"  Type: {module.ModuleType.Name}");
        Console.WriteLine($"  State: {module.State}");
        Console.WriteLine($"  Priority: {module.Priority}");
        Console.WriteLine($"  Description: {module.Description}");
        Console.WriteLine($"  Loaded At: {module.LoadedAt}");
        Console.WriteLine($"  Initialized At: {module.InitializedAt}");
        Console.WriteLine($"  Dependencies: {string.Join(", ", module.Dependencies.Select(d => d.Name))}");
        Console.WriteLine();
    }

    // Use the services
    Console.WriteLine("--- Using Module Services ---\n");

    var logService = app.GetRequiredService<ILogService>();
    logService.LogInfo("This is an info message from the demo app.");
    logService.LogWarning("This is a warning message.");

    var dataRepository = app.GetRequiredService<IDataRepository>();
    await dataRepository.SaveDataAsync("Sample Data 1");
    await dataRepository.SaveDataAsync("Sample Data 2");
    await dataRepository.SaveDataAsync("Sample Data 3");

    var allData = await dataRepository.GetDataAsync();
    Console.WriteLine($"\nRetrieved {allData.Count()} items:");
    foreach (var item in allData)
    {
        Console.WriteLine($"  - {item}");
    }

    Console.WriteLine("\n--- Dependency Graph ---");
    foreach (var module in app.Modules.OrderBy(m => m.Priority).Reverse())
    {
        var dependencyText = module.Dependencies.Any()
            ? $"Depends on {string.Join(", ", module.Dependencies.Select(d => d.Name))}"
            : "No dependencies";
        Console.WriteLine($"{module.Name} (Priority: {module.Priority}) -> {dependencyText}");
    }

    Console.WriteLine("\nInitialization Order: " + 
        string.Join(" -> ", app.Modules.OrderBy(m => m.Priority).Reverse().Select(m => m.Name)));
}

