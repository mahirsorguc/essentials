using HMS.Essentials.Modularity;
using Shouldly;

namespace HMS.Essentials.ObjectMapping;

/// <summary>
/// Tests for <see cref="EssentialsObjectMappingModule"/>.
/// </summary>
public class EssentialsObjectMappingModuleTests
{
    [Fact]
    public void Module_Should_Inherit_From_EssentialsModule()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsObjectMappingModule);

        // Assert
        moduleType.BaseType.ShouldBe(typeof(EssentialsModule));
    }

    [Fact]
    public void Module_Should_Depend_On_CoreModule()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsObjectMappingModule);
        var dependsOnAttribute = moduleType
            .GetCustomAttributes(typeof(DependsOnAttribute), false)
            .FirstOrDefault() as DependsOnAttribute;

        // Assert
        dependsOnAttribute.ShouldNotBeNull();
        dependsOnAttribute!.DependsOn.ShouldContain(typeof(EssentialsCoreModule));
    }

    [Fact]
    public void Module_Should_Be_Public()
    {
        // Arrange & Act
        var moduleType = typeof(EssentialsObjectMappingModule);

        // Assert
        moduleType.IsPublic.ShouldBeTrue();
        moduleType.IsClass.ShouldBeTrue();
    }

    [Fact]
    public void Module_Should_Have_Parameterless_Constructor()
    {
        // Arrange & Act
        var constructor = typeof(EssentialsObjectMappingModule).GetConstructor(Type.EmptyTypes);

        // Assert
        constructor.ShouldNotBeNull();
    }

    [Fact]
    public void Module_Should_Be_Instantiable()
    {
        // Arrange & Act
        var instance = Activator.CreateInstance<EssentialsObjectMappingModule>();

        // Assert
        instance.ShouldNotBeNull();
        instance.ShouldBeOfType<EssentialsObjectMappingModule>();
    }
}
