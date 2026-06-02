
using LabMS.Contracts.TestOrder;

namespace LabMS.Validators;

public class TestOrderCreateRequestValidator : AbstractValidator<TestOrderCreateRequest>
{
    public TestOrderCreateRequestValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.LabTestName)
            .NotEmpty().WithMessage("Lab test name is required")
            .MaximumLength(200).WithMessage("Lab test name cannot exceed 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-&,()]+$").WithMessage("Lab test name can only contain letters, numbers, spaces, hyphens, ampersands, commas, and parentheses");
    }
}
