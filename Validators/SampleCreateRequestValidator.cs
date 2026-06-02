namespace LabMS.Validators;

public class SampleCreateRequestValidator : AbstractValidator<SampleCreateRequest>
{
    public SampleCreateRequestValidator()
    {
        RuleFor(x => x.SampleCode)
            .NotEmpty().WithMessage("Sample code is required")
            .MaximumLength(50).WithMessage("Sample code cannot exceed 50 characters");

        RuleFor(x => x.TestOrderId)
            .NotEmpty().WithMessage("Test order ID is required");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid sample type");

        RuleFor(x => x.CollectedByUserId)
            .NotEmpty().WithMessage("Collected by user ID is required");

        RuleFor(x => x.StorageLocation)
            .MaximumLength(200).WithMessage("Storage location cannot exceed 200 characters");

        RuleFor(x => x.Container)
            .MaximumLength(100).WithMessage("Container cannot exceed 100 characters");

        RuleFor(x => x.Volume)
            .GreaterThan(0).When(x => x.Volume.HasValue)
            .WithMessage("Volume must be greater than 0");

        RuleFor(x => x.VolumeUnit)
            .MaximumLength(20).WithMessage("Volume unit cannot exceed 20 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
    }
}
