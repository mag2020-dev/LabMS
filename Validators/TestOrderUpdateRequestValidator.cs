
using LabMS.Contracts.TestOrder;

namespace LabMS.Validators;

public class TestOrderUpdateRequestValidator : AbstractValidator<TestOrderUpdateRequest>
{
    public TestOrderUpdateRequestValidator()
    {
        RuleFor(x => x.LabTestName)
            .MaximumLength(200).WithMessage("Lab test name cannot exceed 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-&,()]+$").WithMessage("Lab test name can only contain letters, numbers, spaces, hyphens, ampersands, commas, and parentheses")
            .When(x => !string.IsNullOrEmpty(x.LabTestName));

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => status is "Ordered" or "InProgress" or "Completed" or "Cancelled")
            .WithMessage("Status must be one of: Ordered, InProgress, Completed, Cancelled");
    }
}
