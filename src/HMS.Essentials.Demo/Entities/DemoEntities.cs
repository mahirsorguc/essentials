using HMS.Essentials.Domain.Entities;

namespace HMS.Essentials.Entities;

/// <summary>
/// Sample product entity demonstrating integer key usage.
/// </summary>
public class Product : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public override string ToString()
    {
        return $"{Name} - ${Price:F2} (Stock: {Stock})";
    }
}

/// <summary>
/// Sample customer entity demonstrating GUID key usage.
/// </summary>
public class Customer : GuidEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public List<Order> Orders { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}";

    public override string ToString()
    {
        return $"{FullName} ({Email})";
    }
}

/// <summary>
/// Sample order entity demonstrating entity relationships.
/// </summary>
public class Order : GuidEntity
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    public override string ToString()
    {
        return $"Order {Id} - ${TotalAmount:F2} ({Status})";
    }
}

/// <summary>
/// Sample order item entity.
/// </summary>
public class OrderItem : Entity<int>
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;

    public override string ToString()
    {
        return $"{Quantity}x Product #{ProductId} = ${Subtotal:F2}";
    }
}
