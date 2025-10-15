using HMS.Essentials.Modularity;
using HMS.Essentials.Swashbuckle.Extensions;
using HMS.MainApp.WebApi;

var builder = WebApplication.CreateBuilder(args);



// Add HMS Essentials module system to the application
builder.AddModuleSystem<MainAppWebApiModule>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use Swagger documentation
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

// Map controller endpoints
app.MapControllers();

// Get the ApplicationHost to demonstrate loaded modules
var appHost = app.GetApplicationHost();
await DemonstrateLoadedModules(appHost);

app.Run();

static async Task DemonstrateLoadedModules(ApplicationHost app)
{
    Console.WriteLine("\n┌─ MODULE INFORMATION ─────────────────────────────────────┐");
    Console.WriteLine("\nLoaded Modules:");
    foreach (var module in app.Modules.OrderBy(m => m.Priority).Reverse())
    {
        Console.WriteLine($"  • {module.Name} (Priority: {module.Priority}, State: {module.State})");
    }
}