using HMS.Essential.Logging;
using HMS.Essentials;
using HMS.Essentials.Modularity;
using HMS.Essentials.Data;
using HMS.Essentials.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== HMS Essentials Repository Pattern Demo ===\n");

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

// Create service collection
var services = new ServiceCollection();

// Build the application with the root module
// Only specify the top-level module - dependencies are loaded automatically
var app = new ApplicationBuilder(services, configuration)
    .WithEnvironment("Development")
    .UseRootModule<EssentialsDemoModule>()
    .ConfigureServices((svc, cfg) =>
    {
        Console.WriteLine("[Application] Configuring additional services...");
    })
    .Build();

Console.WriteLine("\n=== Application Built Successfully ===\n");

// Demonstrate repository pattern
await DemonstrateRepositoryPatternAsync(app);

Console.WriteLine("\n=== Shutting Down ===\n");

// Cleanup
app.Dispose();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

static async Task DemonstrateRepositoryPatternAsync(ApplicationHost app)
{
    Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║       REPOSITORY PATTERN DEMONSTRATION                     ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

    var productService = app.GetRequiredService<ProductService>();
    var orderService = app.GetRequiredService<OrderService>();

    // Product Management Demo
    Console.WriteLine("┌─ PRODUCT MANAGEMENT ─────────────────────────────────────┐");
    
    Console.WriteLine("\n1. Creating products...");
    var laptop = await productService.CreateProductAsync("Gaming Laptop", 1299.99m, 15, "Electronics");
    var mouse = await productService.CreateProductAsync("Wireless Mouse", 29.99m, 50, "Electronics");
    var keyboard = await productService.CreateProductAsync("Mechanical Keyboard", 89.99m, 30, "Electronics");
    var monitor = await productService.CreateProductAsync("4K Monitor", 499.99m, 20, "Electronics");
    var chair = await productService.CreateProductAsync("Ergonomic Chair", 299.99m, 10, "Furniture");
    
    Console.WriteLine($"  ✓ Created: {laptop}");
    Console.WriteLine($"  ✓ Created: {mouse}");
    Console.WriteLine($"  ✓ Created: {keyboard}");
    Console.WriteLine($"  ✓ Created: {monitor}");
    Console.WriteLine($"  ✓ Created: {chair}");

    Console.WriteLine("\n2. Retrieving all products...");
    var allProducts = await productService.GetAllProductsAsync();
    Console.WriteLine($"  ✓ Total products: {allProducts.Count}");
    foreach (var p in allProducts)
    {
        Console.WriteLine($"    • {p}");
    }

    Console.WriteLine("\n3. Filtering products by category...");
    var electronics = await productService.GetProductsByCategoryAsync("Electronics");
    Console.WriteLine($"  ✓ Electronics: {electronics.Count} items");

    Console.WriteLine("\n4. Finding expensive products (>= $300)...");
    var expensive = await productService.GetExpensiveProductsAsync(300);
    foreach (var p in expensive)
    {
        Console.WriteLine($"  💰 {p}");
    }

    Console.WriteLine("\n5. Updating product price...");
    await productService.UpdateProductPriceAsync(mouse.Id, 24.99m);
    Console.WriteLine($"  ✓ Mouse price updated to $24.99");

    Console.WriteLine("\n6. Stock management - Transferring stock...");
    await productService.AdjustStockAsync(laptop.Id, -5);
    Console.WriteLine($"  ✓ Sold 5 laptops");
    await productService.TransferStockAsync(keyboard.Id, mouse.Id, 10);

    Console.WriteLine("\n7. Pagination demo...");
    var (pagedProducts, totalCount) = await productService.GetPagedProductsAsync(page: 1, pageSize: 3);
    Console.WriteLine($"  ✓ Page 1 of products (showing 3 of {totalCount}):");
    foreach (var p in pagedProducts)
    {
        Console.WriteLine($"    • {p}");
    }

    // Customer and Order Management Demo
    Console.WriteLine("\n┌─ CUSTOMER & ORDER MANAGEMENT ────────────────────────────┐");

    Console.WriteLine("\n8. Creating customers...");
    var customer1 = await orderService.CreateCustomerAsync("John", "Doe", "john.doe@example.com");
    var customer2 = await orderService.CreateCustomerAsync("Jane", "Smith", "jane.smith@example.com");
    Console.WriteLine($"  ✓ Created: {customer1}");
    Console.WriteLine($"  ✓ Created: {customer2}");

    Console.WriteLine("\n9. Creating orders...");
    try
    {
        var order1Items = new List<(int, int)>
        {
            (laptop.Id, 1),
            (mouse.Id, 2),
            (keyboard.Id, 1)
        };
        var order1 = await orderService.CreateOrderAsync(customer1.Id, order1Items);
        Console.WriteLine($"  ✓ Order created: Total ${order1.TotalAmount:F2}");
        Console.WriteLine($"    Items: {order1.Items.Count}");
        foreach (var item in order1.Items)
        {
            Console.WriteLine($"      • {item}");
        }

        var order2Items = new List<(int, int)>
        {
            (monitor.Id, 1),
            (chair.Id, 1)
        };
        var order2 = await orderService.CreateOrderAsync(customer2.Id, order2Items);
        Console.WriteLine($"\n  ✓ Order created: Total ${order2.TotalAmount:F2}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ✗ Order creation failed: {ex.Message}");
    }

    Console.WriteLine("\n10. Retrieving customer orders...");
    var johnOrders = await orderService.GetCustomerOrdersAsync(customer1.Id);
    Console.WriteLine($"  ✓ {customer1.FullName} has {johnOrders.Count} order(s)");

    Console.WriteLine("\n11. Finding customer by email...");
    var foundCustomer = await orderService.FindCustomerByEmailAsync("jane.smith@example.com");
    if (foundCustomer != null)
    {
        Console.WriteLine($"  ✓ Found: {foundCustomer}");
    }

    Console.WriteLine("\n12. Checking low stock products...");
    var lowStock = await productService.GetLowStockProductsAsync(15);
    Console.WriteLine($"  ⚠ Products with stock <= 15: {lowStock.Count}");
    foreach (var p in lowStock)
    {
        Console.WriteLine($"    • {p}");
    }

    Console.WriteLine("\n┌─ MODULE INFORMATION ─────────────────────────────────────┐");
    Console.WriteLine("\nLoaded Modules:");
    foreach (var module in app.Modules.OrderBy(m => m.Priority).Reverse())
    {
        Console.WriteLine($"  • {module.Name} (Priority: {module.Priority}, State: {module.State})");
    }

    Console.WriteLine("\n└──────────────────────────────────────────────────────────┘");
    Console.WriteLine("\n✓ Repository Pattern demonstration completed successfully!");
}


