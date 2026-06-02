
using LabMS.Contracts.TestResult;

namespace LabMS.Validators;

public class TestResultVerifyRequestValidator : AbstractValidator<TestResultVerifyRequest>
{
    public TestResultVerifyRequestValidator()
    {
        RuleFor(x => x.VerifiedById)
            .NotEmpty().WithMessage("Verified by user ID is required");
    }
}
