using HMS.Essentials.Domain.Repositories;
using HMS.Essentials.Entities;

namespace HMS.Essentials.Services;

/// <summary>
/// Service demonstrating repository pattern usage for customer and order management.
/// </summary>
public class OrderService
{
    private readonly IRepository<Customer, Guid> _customerRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<Product, int> _productRepository;

    public OrderService(
        IRepository<Customer, Guid> customerRepository,
        IRepository<Order, Guid> orderRepository,
        IRepository<Product, int> productRepository)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Customer> CreateCustomerAsync(string firstName, string lastName, string email)
    {
        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };

        return await _customerRepository.InsertAsync(customer, autoSave: true);
    }

    public async Task<Order> CreateOrderAsync(Guid customerId, List<(int ProductId, int Quantity)> items)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new InvalidOperationException("Customer not found");

        var order = new Order
        {
            CustomerId = customerId,
            Status = "Pending"
        };

        decimal totalAmount = 0;

        foreach (var (productId, quantity) in items)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException($"Product {productId} not found");

            if (product.Stock < quantity)
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            };

            order.Items.Add(orderItem);
            totalAmount += orderItem.Subtotal;

            // Reduce stock
            product.Stock -= quantity;
            await _productRepository.UpdateAsync(product);
        }

        order.TotalAmount = totalAmount;
        await _orderRepository.InsertAsync(order, autoSave: true);

        Console.WriteLine($"✓ Order created: {order.Id} for customer {customer.FullName}");
        return order;
    }

    public async Task<List<Order>> GetCustomerOrdersAsync(Guid customerId)
    {
        return await _orderRepository.GetListAsync(o => o.CustomerId == customerId);
    }

    public async Task<List<Order>> GetPendingOrdersAsync()
    {
        return await _orderRepository.GetListAsync(o => o.Status == "Pending");
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return false;

        order.Status = newStatus;
        await _orderRepository.UpdateAsync(order, autoSave: true);
        return true;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _customerRepository.GetAllAsync();
    }

    public async Task<Customer?> FindCustomerByEmailAsync(string email)
    {
        return await _customerRepository.GetAsync(c => c.Email == email);
    }
}
