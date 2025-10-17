using FluentValidation;

namespace HMS.MainApp.Samples;

public class CreateSampleDtoValidator : AbstractValidator<CreateSampleDto>
{
    public CreateSampleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.IsActive)
            .NotNull()
            .WithMessage("IsActive must be specified.");
    }
}