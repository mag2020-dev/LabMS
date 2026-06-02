

namespace LabMS.Validators;

public class ReportUpdateRequestValidator : AbstractValidator<ReportUpdateRequest>
{
    public ReportUpdateRequestValidator()
    {
        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("File path is required")
            .MaximumLength(500).WithMessage("File path cannot exceed 500 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_./\\]+$").WithMessage("File path contains invalid characters");
    }
}
