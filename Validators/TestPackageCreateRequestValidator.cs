namespace LabMS.Validators;

public class TestPackageCreateRequestValidator : AbstractValidator<TestPackageCreateRequest>
{
    public TestPackageCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Package name is required")
            .MaximumLength(200).WithMessage("Package name cannot exceed 200 characters");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Package code cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.PackagePrice)
            .GreaterThan(0).WithMessage("Package price must be greater than 0");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid package type");

        RuleFor(x => x.Tests)
            .NotEmpty().WithMessage("Package must contain at least one test")
            .Must(tests => tests.Count > 0).WithMessage("Package must contain at least one test");

        RuleForEach(x => x.Tests).ChildRules(item =>
        {
            item.RuleFor(x => x.LabTestId)
                .NotEmpty().WithMessage("Test ID is required");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");
        });
    }
}
