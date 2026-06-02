namespace LabMS.Validators;

public class PackageSubscriptionCreateRequestValidator : AbstractValidator<PackageSubscriptionCreateRequest>
{
    public PackageSubscriptionCreateRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.TestPackageId)
            .NotEmpty().WithMessage("Test package ID is required");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(DateTime.Today).WithMessage("End date must be in the future");

        RuleFor(x => x.Frequency)
            .IsInEnum().WithMessage("Invalid subscription frequency");

        RuleFor(x => x.TestsIncluded)
            .GreaterThan(0).WithMessage("Tests included must be greater than 0");
    }
}
