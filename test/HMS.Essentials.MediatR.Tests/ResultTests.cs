using Shouldly;

namespace HMS.Essentials.MediatR.Tests;

public class ResultTests
{
    [Fact]
    public void Success_WithValue_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(value);
        result.Error.ShouldBeNull();
        result.Errors.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithSingleError_ShouldCreateFailedResult()
    {
        // Arrange
        var error = "test error";

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Error.ShouldBe(error);
        result.Errors.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithMultipleErrors_AsCollection_ShouldCreateFailedResult()
    {
        // Arrange
        var errors = new List<string> { "error 1", "error 2", "error 3" };

        // Act
        var result = Result<string>.Failure(errors);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Error.ShouldBeNull();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldBe(errors);
        result.Errors.Count.ShouldBe(3);
    }

    [Fact]
    public void Failure_WithMultipleErrors_AsParams_ShouldCreateFailedResult()
    {
        // Act
        var result = Result<string>.Failure("error 1", "error 2", "error 3");

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Error.ShouldBeNull();
        result.Errors.ShouldNotBeNull();
        result.Errors.Count.ShouldBe(3);
        result.Errors.ShouldContain("error 1");
        result.Errors.ShouldContain("error 2");
        result.Errors.ShouldContain("error 3");
    }

    [Fact]
    public void Success_WithoutValue_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBeNull();
        result.Errors.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithoutValue_WithSingleError_ShouldCreateFailedResult()
    {
        // Arrange
        var error = "test error";

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
        result.Errors.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithoutValue_WithMultipleErrors_AsCollection_ShouldCreateFailedResult()
    {
        // Arrange
        var errors = new List<string> { "error 1", "error 2" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldBe(errors);
    }

    [Fact]
    public void Failure_WithoutValue_WithMultipleErrors_AsParams_ShouldCreateFailedResult()
    {
        // Act
        var result = Result.Failure("error 1", "error 2");

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.Errors.ShouldNotBeNull();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void Success_WithComplexType_ShouldWorkCorrectly()
    {
        // Arrange
        var user = new TestUser { Id = 1, Name = "John Doe" };

        // Act
        var result = Result<TestUser>.Success(user);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(1);
        result.Value.Name.ShouldBe("John Doe");
    }

    [Fact]
    public void Failure_WithComplexType_ShouldWorkCorrectly()
    {
        // Act
        var result = Result<TestUser>.Failure("User not found");

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error.ShouldBe("User not found");
    }

    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
