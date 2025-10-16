using HMS.Essentials.AspNetCore.Serilog.Extensions;
using HMS.Essentials.Modularity;
using HMS.Essentials.Swashbuckle.Extensions;
using HMS.MainApp.WebApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging (must be called before AddModuleSystem)
builder.AddSerilogLogging();

// Add HMS Essentials module system to the application
builder.AddModuleSystem<MainAppWebApiModule>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use Swagger documentation
    app.UseSwaggerDocumentation();
}

// Use Serilog request logging
app.UseEssentialsSerilogRequestLogging();

app.UseHttpsRedirection();

// Map controller endpoints
app.MapControllers();

// Get the ApplicationHost to demonstrate loaded modules
var appHost = app.GetApplicationHost();
await DemonstrateLoadedModules(appHost);

try
{
    Log.Information("Starting HMS Main Application Web API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static async Task DemonstrateLoadedModules(ApplicationHost app)
{
    Console.WriteLine("\n┌─ MODULE INFORMATION ─────────────────────────────────────┐");
    Console.WriteLine("\nLoaded Modules:");
    foreach (var module in app.Modules.OrderBy(m => m.Priority).Reverse())
    {
        Console.WriteLine($"  • {module.Name} (Priority: {module.Priority}, State: {module.State})");
    }
}