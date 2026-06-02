
using LabMS.Contracts.LabTest;

namespace LabMS.Validators;

public class LabTestCreateRequestValidator : AbstractValidator<LabTestCreateRequest>
{
    public LabTestCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Test name is required")
            .MaximumLength(200).WithMessage("Test name cannot exceed 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-&,()]+$").WithMessage("Test name can only contain letters, numbers, spaces, hyphens, ampersands, commas, and parentheses");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Test code cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9\-_]+$").WithMessage("Test code can only contain uppercase letters, numbers, hyphens, and underscores")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThan(10000).WithMessage("Price cannot exceed 10,000");

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0).WithMessage("Estimated duration must be greater than 0")
            .LessThanOrEqualTo(168).WithMessage("Estimated duration cannot exceed 168 hours (1 week)")
            .When(x => x.EstimatedDuration.HasValue);
    }
}
