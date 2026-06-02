
using LabMS.Contracts.Visit;

namespace LabMS.Validators;

public class VisitUpdateRequestValidator : AbstractValidator<VisitUpdateRequest>
{
    public VisitUpdateRequestValidator()
    {
        RuleFor(x => x.DoctorName)
            .MaximumLength(200).WithMessage("Doctor name cannot exceed 200 characters")
            .Matches(@"^[a-zA-Z\s\-'.]+$").WithMessage("Doctor name can only contain letters, spaces, hyphens, apostrophes, and periods")
            .When(x => !string.IsNullOrEmpty(x.DoctorName));

        RuleFor(x => x.VisitDate)
            .NotEmpty().WithMessage("Visit date is required")
            .GreaterThanOrEqualTo(DateTime.Today.AddDays(-30)).WithMessage("Visit date cannot be more than 30 days in the past")
            .LessThanOrEqualTo(DateTime.Today.AddDays(365)).WithMessage("Visit date cannot be more than 1 year in the future");

        RuleFor(x => x.Reason)
            .MaximumLength(1000).WithMessage("Reason cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid visit status");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
