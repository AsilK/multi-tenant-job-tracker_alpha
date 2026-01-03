using FluentValidation;

namespace JobApplicationTracker.Application.Features.Jobs.Commands.CreateJob;

/// <summary>
/// Validator for CreateJobCommand.
/// </summary>
public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(10000).WithMessage("Description cannot exceed 10000 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department cannot exceed 100 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid job type.");

        RuleFor(x => x.MinSalary)
            .GreaterThanOrEqualTo(0).When(x => x.MinSalary.HasValue)
            .WithMessage("Minimum salary cannot be negative.");

        RuleFor(x => x.MaxSalary)
            .GreaterThanOrEqualTo(x => x.MinSalary ?? 0).When(x => x.MaxSalary.HasValue)
            .WithMessage("Maximum salary must be greater than or equal to minimum salary.");

        RuleFor(x => x.ClosingDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.ClosingDate.HasValue)
            .WithMessage("Closing date must be in the future.");
    }
}
