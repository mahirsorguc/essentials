using HMS.Essentials.Modularity;
using Shouldly;

namespace HMS.Essentials.AutoMapper;

/// <summary>
/// Tests for <see cref="EssentialsAutomapperModule"/>.
/// </summary>
public class EssentialsAutomapperModuleTests
{
    [Fact]
    public void Module_Should_Inherit_From_EssentialsModule()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAutomapperModule);

        // Assert
        moduleType.BaseType.ShouldBe(typeof(EssentialsModule));
    }

    [Fact]
    public void Module_Should_Depend_On_ObjectMappingModule()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAutomapperModule);
        var dependsOnAttribute = moduleType
            .GetCustomAttributes(typeof(DependsOnAttribute), false)
            .FirstOrDefault() as DependsOnAttribute;

        // Assert
        dependsOnAttribute.ShouldNotBeNull();
        dependsOnAttribute!.DependsOn.ShouldContain(typeof(HMS.Essentials.ObjectMapping.EssentialsObjectMappingModule));
    }

    [Fact]
    public void Module_Should_Be_Public()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAutomapperModule);

        // Assert
        moduleType.IsPublic.ShouldBeTrue();
        moduleType.IsClass.ShouldBeTrue();
    }

    [Fact]
    public void Module_Should_Have_Parameterless_Constructor()
    {
        // Arrange & Act
        var constructor = typeof(EssentialsAutomapperModule).GetConstructor(Type.EmptyTypes);

        // Assert
        constructor.ShouldNotBeNull();
    }

    [Fact]
    public void Module_Should_Be_Instantiable()
    {
        // Arrange & Act
        var instance = Activator.CreateInstance<EssentialsAutomapperModule>();

        // Assert
        instance.ShouldNotBeNull();
        instance.ShouldBeOfType<EssentialsAutomapperModule>();
    }

    [Fact]
    public void Module_Should_Have_Correct_Namespace()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsAutomapperModule);

        // Assert
        moduleType.Namespace.ShouldBe("HMS.Essentials.AutoMapper");
    }
}
