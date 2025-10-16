using HMS.Essentials.AutoMapper.Extensions;
using HMS.Essentials.AutoMapper.ObjectMapping;
using HMS.Essentials.ObjectMapping;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace HMS.Essentials.AutoMapper;

/// <summary>
/// Integration tests for AutoMapper implementation.
/// </summary>
public class AutoMapperIntegrationTests
{
    [Fact]
    public void End_To_End_Mapping_Should_Work()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(IntegrationTestProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Email = "john.doe@example.com",
            Address = new Address
            {
                Street = "123 Main St",
                City = "Anytown",
                ZipCode = "12345"
            }
        };

        // Act
        var dto = objectMapper.Map<PersonDto>(person);

        // Assert
        dto.ShouldNotBeNull();
        dto.Id.ShouldBe(1);
        dto.FullName.ShouldBe("John Doe");
        dto.Age.ShouldBe(30);
        dto.Email.ShouldBe("john.doe@example.com");
        dto.FullAddress.ShouldBe("123 Main St, Anytown 12345");
    }

    [Fact]
    public void Reverse_Mapping_Should_Work()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(IntegrationTestProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var dto = new SimpleDto { Name = "Test", Value = 42 };

        // Act
        var entity = objectMapper.Map<SimpleEntity>(dto);

        // Assert
        entity.ShouldNotBeNull();
        entity.Name.ShouldBe("Test");
        entity.Value.ShouldBe(42);
    }

    [Fact]
    public void Collection_Mapping_Should_Work()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(IntegrationTestProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var entities = new List<SimpleEntity>
        {
            new() { Name = "Item1", Value = 1 },
            new() { Name = "Item2", Value = 2 },
            new() { Name = "Item3", Value = 3 }
        };

        // Act
        var dtos = objectMapper.Map<List<SimpleDto>>(entities);

        // Assert
        dtos.ShouldNotBeNull();
        dtos.Count.ShouldBe(3);
        dtos[0].Name.ShouldBe("Item1");
        dtos[1].Value.ShouldBe(2);
    }

    [Fact]
    public void Mapping_To_Existing_Object_Should_Work()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(IntegrationTestProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var source = new SimpleEntity { Name = "Updated", Value = 100 };
        var destination = new SimpleDto { Name = "Original", Value = 1 };

        // Act
        var result = objectMapper.Map(source, destination);

        // Assert
        result.ShouldBeSameAs(destination);
        result.Name.ShouldBe("Updated");
        result.Value.ShouldBe(100);
    }

    [Fact]
    public void Nested_Object_Mapping_Should_Work()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(IntegrationTestProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var order = new Order
        {
            Id = 1,
            OrderNumber = "ORD-001",
            Customer = new Customer
            {
                Id = 100,
                Name = "Jane Smith",
                Email = "jane@example.com"
            },
            Items = new List<OrderItem>
            {
                new() { ProductName = "Product A", Quantity = 2, Price = 10.50m },
                new() { ProductName = "Product B", Quantity = 1, Price = 25.00m }
            }
        };

        // Act
        var dto = objectMapper.Map<OrderDto>(order);

        // Assert
        dto.ShouldNotBeNull();
        dto.Id.ShouldBe(1);
        dto.OrderNumber.ShouldBe("ORD-001");
        dto.CustomerName.ShouldBe("Jane Smith");
        dto.CustomerEmail.ShouldBe("jane@example.com");
        dto.Items.Count.ShouldBe(2);
        dto.Items[0].ProductName.ShouldBe("Product A");
        dto.TotalAmount.ShouldBe(46.00m);
    }

    [Fact]
    public void Null_Source_Should_Return_Null()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(IntegrationTestProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();

        // Act
        var result = objectMapper.Map<SimpleDto>((SimpleEntity)null!);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Custom_Value_Resolver_Should_Work()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(typeof(IntegrationTestProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30
        };

        // Act
        var dto = objectMapper.Map<PersonDto>(person);

        // Assert
        dto.FullName.ShouldBe("John Doe");
    }

    [Fact]
    public void Multiple_Profiles_Should_Work_Together()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEssentialsAutoMapper(
            typeof(IntegrationTestProfile),
            typeof(AdditionalProfile));
        var serviceProvider = services.BuildServiceProvider();
        
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();

        // Act
        var dto1 = objectMapper.Map<SimpleDto>(new SimpleEntity { Name = "Test" });
        var dto2 = objectMapper.Map<AnotherDto>(new AnotherEntity { Data = "Data" });

        // Assert
        dto1.ShouldNotBeNull();
        dto2.ShouldNotBeNull();
    }

    // Test Profile
    private class IntegrationTestProfile : EssentialsProfile
    {
        protected override void ConfigureMappings()
        {
            CreateMap<Person, PersonDto>()
                .ForMember(dest => dest.FullName, 
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.FullAddress,
                    opt => opt.MapFrom(src => $"{src.Address.Street}, {src.Address.City} {src.Address.ZipCode}"));

            CreateMap<SimpleEntity, SimpleDto>().ReverseMap();

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, 
                    opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerEmail,
                    opt => opt.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.MapFrom(src => src.Items.Sum(i => i.Price * i.Quantity)));

            CreateMap<OrderItem, OrderItemDto>();
        }
    }

    private class AdditionalProfile : EssentialsProfile
    {
        protected override void ConfigureMappings()
        {
            CreateMap<AnotherEntity, AnotherDto>();
        }
    }

    // Test entities
    private class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public Address Address { get; set; } = null!;
    }

    private class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    private class PersonDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
    }

    private class SimpleEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class SimpleDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Customer Customer { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new();
    }

    private class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private class OrderItem
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    private class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }

    private class OrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    private class AnotherEntity
    {
        public string Data { get; set; } = string.Empty;
    }

    private class AnotherDto
    {
        public string Data { get; set; } = string.Empty;
    }
}
