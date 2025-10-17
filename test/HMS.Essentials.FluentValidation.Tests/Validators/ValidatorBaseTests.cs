using FluentValidation;
using HMS.Essentials.FluentValidation.Validators;

namespace HMS.Essentials.FluentValidation.Validators;

/// <summary>
/// Tests for <see cref="ValidatorBase{T}"/>.
/// </summary>
public class ValidatorBaseTests
{
    private class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public List<string> Tags { get; set; } = new();
        public string? Email { get; set; }
    }

    private class TestModelValidator : ValidatorBase<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Name)
                .Must(BeNonEmptyString)
                .WithMessage("Name cannot be empty");

            RuleFor(x => x.Id)
                .Must(BeNonEmptyGuid)
                .WithMessage("Id cannot be empty");

            RuleFor(x => x.Age)
                .Must(age => BeInRange(age, 0, 120))
                .WithMessage("Age must be between 0 and 120");

            RuleFor(x => x.BirthDate)
                .Must(BeInPast)
                .WithMessage("Birth date must be in the past");

            RuleFor(x => x.LastLogin)
                .Must(date => date == null || BeNotInPast(date.Value))
                .WithMessage("Last login cannot be in the past");

            RuleFor(x => x.Tags)
                .Must(HaveItems)
                .WithMessage("Tags cannot be empty")
                .Must(HaveNoDuplicates)
                .WithMessage("Tags must be unique");

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || MatchPattern(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                .WithMessage("Invalid email format");
        }
    }

    [Fact]
    public void ValidatorBase_Should_Validate_Valid_Model()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel
        {
            Name = "John Doe",
            Id = Guid.NewGuid(),
            Age = 25,
            BirthDate = DateTime.Now.AddYears(-25),
            Tags = new List<string> { "tag1", "tag2" },
            Email = "john@example.com"
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void ValidatorBase_Should_Detect_Empty_String()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel
        {
            Name = "",
            Id = Guid.NewGuid(),
            Age = 25,
            BirthDate = DateTime.Now.AddYears(-25),
            Tags = new List<string> { "tag1" }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(TestModel.Name));
    }

    [Fact]
    public void ValidatorBase_Should_Detect_Empty_Guid()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel
        {
            Name = "John",
            Id = Guid.Empty,
            Age = 25,
            BirthDate = DateTime.Now.AddYears(-25),
            Tags = new List<string> { "tag1" }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(TestModel.Id));
    }

    [Fact]
    public void ValidatorBase_Should_Validate_Range()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel
        {
            Name = "John",
            Id = Guid.NewGuid(),
            Age = 150,
            BirthDate = DateTime.Now.AddYears(-25),
            Tags = new List<string> { "tag1" }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(TestModel.Age));
    }

    [Fact]
    public void ValidatorBase_Should_Detect_Duplicate_Items()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel
        {
            Name = "John",
            Id = Guid.NewGuid(),
            Age = 25,
            BirthDate = DateTime.Now.AddYears(-25),
            Tags = new List<string> { "tag1", "tag2", "tag1" }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(TestModel.Tags));
    }

    [Fact]
    public void ValidatorBase_Should_Validate_Email_Pattern()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel
        {
            Name = "John",
            Id = Guid.NewGuid(),
            Age = 25,
            BirthDate = DateTime.Now.AddYears(-25),
            Tags = new List<string> { "tag1" },
            Email = "invalid-email"
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(TestModel.Email));
    }
}
