using FluentValidation.Results;
using HMS.Essentials.FluentValidation.Extensions;

namespace HMS.Essentials.FluentValidation.Extensions;

/// <summary>
/// Tests for <see cref="ValidationResultExtensions"/>.
/// </summary>
public class ValidationResultExtensionsTests
{
    [Fact]
    public void ToDictionary_Should_Convert_Errors_To_Dictionary()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid"),
            new ValidationFailure("Email", "Email is already taken")
        });

        // Act
        var dictionary = validationResult.ToDictionary();

        // Assert
        dictionary.ShouldNotBeNull();
        dictionary.Count.ShouldBe(2);
        dictionary.ShouldContainKey("Name");
        dictionary.ShouldContainKey("Email");
        
        var nameErrors = dictionary["Name"];
        nameErrors.ShouldNotBeEmpty();
        nameErrors.ShouldHaveSingleItem();
        nameErrors[0].ShouldBe("Name is required");
        
        var emailErrors = dictionary["Email"];
        emailErrors.ShouldNotBeEmpty();
        emailErrors.Count().ShouldBe(2);
        emailErrors[0].ShouldBe("Email is invalid");
        emailErrors[1].ShouldBe("Email is already taken");
    }

    [Fact]
    public void ToDictionary_Should_Return_Empty_Dictionary_When_No_Errors()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var dictionary = validationResult.ToDictionary();

        // Assert
        dictionary.ShouldNotBeNull();
        dictionary.ShouldBeEmpty();
    }

    [Fact]
    public void ToErrorString_Should_Format_Errors_As_String()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        });

        // Act
        var errorString = validationResult.ToErrorString();

        // Assert
        errorString.ShouldNotBeNullOrWhiteSpace();
        errorString.ShouldContain("Name is required");
        errorString.ShouldContain("Email is invalid");
    }

    [Fact]
    public void ToErrorString_Should_Use_Custom_Separator()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        });

        // Act
        var errorString = validationResult.ToErrorString(" | ");

        // Assert
        errorString.ShouldNotBeNullOrWhiteSpace();
        errorString.ShouldContain(" | ");
        errorString.ShouldContain("Name is required");
        errorString.ShouldContain("Email is invalid");
    }

    [Fact]
    public void ToErrorString_Should_Return_Empty_String_When_No_Errors()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var errorString = validationResult.ToErrorString();

        // Assert
        errorString.ShouldBe(string.Empty);
    }

    [Fact]
    public void GetErrorsForProperty_Should_Return_Errors_For_Specific_Property()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid"),
            new ValidationFailure("Email", "Email is already taken"),
            new ValidationFailure("Age", "Age must be positive")
        });

        // Act
        var emailErrors = validationResult.GetErrorsForProperty("Email");

        // Assert
        emailErrors.ShouldNotBeNull();
        emailErrors.Count.ShouldBe(2);
        emailErrors[0].ShouldBe("Email is invalid");
        emailErrors[1].ShouldBe("Email is already taken");
    }

    [Fact]
    public void GetErrorsForProperty_Should_Return_Empty_List_When_Property_Has_No_Errors()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        // Act
        var emailErrors = validationResult.GetErrorsForProperty("Email");

        // Assert
        emailErrors.ShouldNotBeNull();
        emailErrors.ShouldBeEmpty();
    }

    [Fact]
    public void HasErrorsForProperty_Should_Return_True_When_Property_Has_Errors()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        });

        // Act
        var hasEmailErrors = validationResult.HasErrorsForProperty("Email");

        // Assert
        hasEmailErrors.ShouldBeTrue();
    }

    [Fact]
    public void HasErrorsForProperty_Should_Return_False_When_Property_Has_No_Errors()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        // Act
        var hasEmailErrors = validationResult.HasErrorsForProperty("Email");

        // Assert
        hasEmailErrors.ShouldBeFalse();
    }

    [Fact]
    public void ToJsonObject_Should_Convert_To_Json_Object()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        });

        // Act
        var jsonObject = validationResult.ToJsonObject();

        // Assert
        jsonObject.ShouldNotBeNull();
        
        // Use reflection to verify the structure
        var jsonType = jsonObject.GetType();
        var isValidProperty = jsonType.GetProperty("IsValid");
        var errorsProperty = jsonType.GetProperty("Errors");
        
        isValidProperty.ShouldNotBeNull();
        errorsProperty.ShouldNotBeNull();
        
        var isValid = (bool)isValidProperty!.GetValue(jsonObject)!;
        isValid.ShouldBeFalse();
        
        var errors = errorsProperty!.GetValue(jsonObject) as System.Collections.IEnumerable;
        errors.ShouldNotBeNull();
        errors.Cast<object>().Count().ShouldBe(2);
    }

    [Fact]
    public void ToJsonObject_Should_Return_Empty_Errors_When_No_Errors()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var jsonObject = validationResult.ToJsonObject();

        // Assert
        jsonObject.ShouldNotBeNull();
        
        // Use reflection to verify the structure
        var jsonType = jsonObject.GetType();
        var isValidProperty = jsonType.GetProperty("IsValid");
        var errorsProperty = jsonType.GetProperty("Errors");
        
        isValidProperty.ShouldNotBeNull();
        errorsProperty.ShouldNotBeNull();
        
        var isValid = (bool)isValidProperty!.GetValue(jsonObject)!;
        isValid.ShouldBeTrue();
        
        var errors = errorsProperty!.GetValue(jsonObject) as System.Collections.IEnumerable;
        errors.ShouldNotBeNull();
        errors.Cast<object>().Count().ShouldBe(0);
    }
}
