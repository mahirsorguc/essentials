using Shouldly;

namespace HMS.Essentials.Modularity;

public class DependsOnAttributeTests
{
    [Fact]
    public void Constructor_With_Single_Dependency_Should_Set_DependsOn()
    {
        // Arrange & Act
        var attribute = new DependsOnAttribute(typeof(ModuleA));

        // Assert
        attribute.DependsOn.ShouldNotBeNull();
        attribute.DependsOn.Length.ShouldBe(1);
        attribute.DependsOn[0].ShouldBe(typeof(ModuleA));
    }

    [Fact]
    public void Constructor_With_Multiple_Dependencies_Should_Set_DependsOn()
    {
        // Arrange & Act
        var attribute = new DependsOnAttribute(typeof(ModuleA), typeof(ModuleB), typeof(ModuleC));

        // Assert
        attribute.DependsOn.ShouldNotBeNull();
        attribute.DependsOn.Length.ShouldBe(3);
        attribute.DependsOn[0].ShouldBe(typeof(ModuleA));
        attribute.DependsOn[1].ShouldBe(typeof(ModuleB));
        attribute.DependsOn[2].ShouldBe(typeof(ModuleC));
    }

    [Fact]
    public void Constructor_With_No_Dependencies_Should_Set_Empty_Array()
    {
        // Arrange & Act
        var attribute = new DependsOnAttribute();

        // Assert
        attribute.DependsOn.ShouldNotBeNull();
        attribute.DependsOn.Length.ShouldBe(0);
    }

    [Fact]
    public void Attribute_Should_Be_Applicable_To_Classes()
    {
        // Arrange
        var attributeUsage = typeof(DependsOnAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.ValidOn.ShouldBe(AttributeTargets.Class);
    }

    [Fact]
    public void Attribute_Should_Not_Allow_Multiple_Instances()
    {
        // Arrange
        var attributeUsage = typeof(DependsOnAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.AllowMultiple.ShouldBeFalse();
    }

    [Fact]
    public void Attribute_Should_Not_Be_Inherited()
    {
        // Arrange
        var attributeUsage = typeof(DependsOnAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.Inherited.ShouldBeFalse();
    }

    [Fact]
    public void Can_Apply_Attribute_To_Module_Class()
    {
        // Arrange & Act
        var attributes = typeof(TestModuleWithDependencies)
            .GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .ToArray();

        // Assert
        attributes.ShouldNotBeNull();
        attributes.Length.ShouldBe(1);
        attributes[0].DependsOn[0].ShouldBe(typeof(ModuleA));
    }

    [Fact]
    public void Can_Apply_Multiple_Dependencies_To_Module_Class()
    {
        // Arrange & Act
        var attributes = typeof(TestModuleWithMultipleDependencies)
            .GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>()
            .ToArray();

        // Assert
        attributes.ShouldNotBeNull();
        attributes.Length.ShouldBe(1);
        attributes[0].DependsOn.Length.ShouldBe(2);
        attributes[0].DependsOn.ShouldContain(typeof(ModuleA));
        attributes[0].DependsOn.ShouldContain(typeof(ModuleB));
    }

    private class ModuleA : EssentialsModule { }
    private class ModuleB : EssentialsModule { }
    private class ModuleC : EssentialsModule { }

    [DependsOn(typeof(ModuleA))]
    private class TestModuleWithDependencies : EssentialsModule { }

    [DependsOn(typeof(ModuleA), typeof(ModuleB))]
    private class TestModuleWithMultipleDependencies : EssentialsModule { }
}
