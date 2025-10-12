using HMS.Essentials.Domain.Entities;
using Shouldly;

namespace HMS.Essentials.Domain.Specifications;

public class SpecificationTests
{
    private class TestProduct : Entity<int>
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    private class ProductByNameSpec : Specification<TestProduct>
    {
        public ProductByNameSpec(string name) 
            : base(p => p.Name == name)
        {
        }
    }

    private class ProductByCategorySpec : Specification<TestProduct>
    {
        public ProductByCategorySpec(string category)
            : base(p => p.Category == category)
        {
        }
    }

    private class ActiveProductSpec : Specification<TestProduct>
    {
        public ActiveProductSpec()
            : base(p => p.IsActive)
        {
        }
    }

    [Fact]
    public void Specification_WithCriteria_ShouldFilterCorrectly()
    {
        // Arrange
        var spec = new ProductByNameSpec("Test Product");
        var products = new[]
        {
            new TestProduct { Name = "Test Product" },
            new TestProduct { Name = "Other Product" }
        };

        // Act
        var result = products.AsQueryable().Where(spec.Criteria!).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Test Product");
    }

    [Fact]
    public void Specification_And_ShouldCombineWithAnd()
    {
        // Arrange
        var categorySpec = new ProductByCategorySpec("Electronics");
        var activeSpec = new ActiveProductSpec();
        var combinedSpec = categorySpec.And(activeSpec);
        
        var products = new[]
        {
            new TestProduct { Category = "Electronics", IsActive = true },
            new TestProduct { Category = "Electronics", IsActive = false },
            new TestProduct { Category = "Books", IsActive = true }
        };

        // Act
        var result = products.AsQueryable().Where(combinedSpec.Criteria!).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Category.ShouldBe("Electronics");
        result[0].IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Specification_Or_ShouldCombineWithOr()
    {
        // Arrange
        var nameSpec = new ProductByNameSpec("Product A");
        var categorySpec = new ProductByCategorySpec("Books");
        var combinedSpec = nameSpec.Or(categorySpec);
        
        var products = new[]
        {
            new TestProduct { Name = "Product A", Category = "Electronics" },
            new TestProduct { Name = "Product B", Category = "Books" },
            new TestProduct { Name = "Product C", Category = "Music" }
        };

        // Act
        var result = products.AsQueryable().Where(combinedSpec.Criteria!).ToList();

        // Assert
        result.Count.ShouldBe(2);
    }

    [Fact]
    public void Specification_Not_ShouldNegate()
    {
        // Arrange
        var activeSpec = new ActiveProductSpec();
        var notActiveSpec = activeSpec.Not();
        
        var products = new[]
        {
            new TestProduct { IsActive = true },
            new TestProduct { IsActive = false }
        };

        // Act
        var result = products.AsQueryable().Where(notActiveSpec.Criteria!).ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].IsActive.ShouldBeFalse();
    }
}
