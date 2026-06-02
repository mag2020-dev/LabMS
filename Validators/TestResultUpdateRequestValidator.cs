
using LabMS.Contracts.TestResult;

namespace LabMS.Validators;

public class TestResultUpdateRequestValidator : AbstractValidator<TestResultUpdateRequest>
{
    public TestResultUpdateRequestValidator()
    {
        RuleFor(x => x.ResultValue)
            .NotEmpty().WithMessage("Result value is required")
            .MaximumLength(200).WithMessage("Result value cannot exceed 200 characters");

        RuleFor(x => x.Unit)
            .MaximumLength(50).WithMessage("Unit cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Unit));

        RuleFor(x => x.ReferenceRange)
            .MaximumLength(250).WithMessage("Reference range cannot exceed 250 characters")
            .When(x => !string.IsNullOrEmpty(x.ReferenceRange));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
