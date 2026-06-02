
using LabMS.Contracts.Doctor;

namespace LabMS.Validators;

public class DoctorCreateRequestValidator : AbstractValidator<DoctorCreateRequest>
{
    public DoctorCreateRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s\-'.]+$").WithMessage("First name can only contain letters, spaces, hyphens, apostrophes, and periods");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s\-'.]+$").WithMessage("Last name can only contain letters, spaces, hyphens, apostrophes, and periods");

        RuleFor(x => x.Specialty)
            .MaximumLength(100).WithMessage("Specialty cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s\-&,]+$").WithMessage("Specialty can only contain letters, spaces, hyphens, ampersands, and commas")
            .When(x => !string.IsNullOrEmpty(x.Specialty));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^[\+]?[1-9][\d]{0,15}$").WithMessage("Phone number must be a valid international format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
