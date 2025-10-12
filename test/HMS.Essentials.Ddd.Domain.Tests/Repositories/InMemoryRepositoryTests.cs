using HMS.Essentials.Domain.Entities;
using HMS.Essentials.Domain.Repositories;
using Shouldly;

namespace HMS.Essentials.Ddd.Domain.Tests.Repositories;

public class InMemoryRepositoryTests
{
    private class TestProduct : Entity<int>
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    [Fact]
    public async Task InsertAsync_ShouldAddEntity()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        var product = new TestProduct { Name = "Test Product", Price = 99.99m };
        var result = await repository.InsertAsync(product);
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        var product = new TestProduct { Name = "Test", Price = 50m };
        await repository.InsertAsync(product, autoSave: true);
        var result = await repository.GetByIdAsync(product.Id);
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        await repository.InsertAsync(new TestProduct { Name = "P1" }, autoSave: true);
        await repository.InsertAsync(new TestProduct { Name = "P2" }, autoSave: true);
        var result = await repository.GetAllAsync();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyEntity()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        var product = new TestProduct { Name = "Original", Price = 100m };
        await repository.InsertAsync(product, autoSave: true);
        product.Name = "Updated";
        await repository.UpdateAsync(product, autoSave: true);
        var retrieved = await repository.GetByIdAsync(product.Id);
        retrieved!.Name.ShouldBe("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        var product = new TestProduct { Name = "To Delete" };
        await repository.InsertAsync(product, autoSave: true);
        await repository.DeleteAsync(product, autoSave: true);
        var count = await repository.CountAsync();
        count.ShouldBe(0);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        await repository.InsertAsync(new TestProduct { Name = "P1" }, autoSave: true);
        await repository.InsertAsync(new TestProduct { Name = "P2" }, autoSave: true);
        await repository.InsertAsync(new TestProduct { Name = "P3" }, autoSave: true);
        var count = await repository.CountAsync();
        count.ShouldBe(3);
    }

    [Fact]
    public async Task InsertManyAsync_ShouldAddMultipleEntities()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        var products = new[]
        {
            new TestProduct { Name = "P1", Price = 10m },
            new TestProduct { Name = "P2", Price = 20m }
        };
        await repository.InsertManyAsync(products, autoSave: true);
        var count = await repository.CountAsync();
        count.ShouldBe(2);
    }

    [Fact]
    public async Task GetAsync_WithPredicate_ShouldReturnFirstMatch()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        await repository.InsertAsync(new TestProduct { Name = "A", Price = 100m }, autoSave: true);
        await repository.InsertAsync(new TestProduct { Name = "B", Price = 200m }, autoSave: true);
        var result = await repository.GetAsync(p => p.Price > 150m);
        result.ShouldNotBeNull();
        result!.Name.ShouldBe("B");
    }

    [Fact]
    public async Task GetListAsync_WithPredicate_ShouldReturnFiltered()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        await repository.InsertAsync(new TestProduct { Name = "Expensive", Price = 500m }, autoSave: true);
        await repository.InsertAsync(new TestProduct { Name = "Cheap", Price = 10m }, autoSave: true);
        var result = await repository.GetListAsync(p => p.Price >= 100m);
        result.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetPagedListAsync_ShouldReturnCorrectPage()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        for (int i = 1; i <= 10; i++)
            await repository.InsertAsync(new TestProduct { Name = $"Product {i}" }, autoSave: true);
        var page1 = await repository.GetPagedListAsync(0, 3);
        var page2 = await repository.GetPagedListAsync(3, 3);
        page1.Count.ShouldBe(3);
        page2.Count.ShouldBe(3);
    }

    [Fact]
    public async Task DeleteManyAsync_ByPredicate_ShouldRemoveMatching()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        await repository.InsertAsync(new TestProduct { Name = "P1", Price = 10m }, autoSave: true);
        await repository.InsertAsync(new TestProduct { Name = "P2", Price = 200m }, autoSave: true);
        await repository.DeleteManyAsync(p => p.Price > 100m, autoSave: true);
        var remaining = await repository.CountAsync();
        remaining.ShouldBe(1);
    }

    [Fact]
    public async Task AnyAsync_WithPredicate_ShouldReturnCorrectResult()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        await repository.InsertAsync(new TestProduct { Price = 100m }, autoSave: true);
        var exists = await repository.AnyAsync(p => p.Price > 50m);
        var notExists = await repository.AnyAsync(p => p.Price > 200m);
        exists.ShouldBeTrue();
        notExists.ShouldBeFalse();
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        var repository = new InMemoryRepository<TestProduct, int>();
        await repository.InsertAsync(new TestProduct { Price = 500m }, autoSave: true);
        await repository.InsertAsync(new TestProduct { Price = 10m }, autoSave: true);
        var count = await repository.CountAsync(p => p.Price >= 500m);
        count.ShouldBe(1);
    }
}
