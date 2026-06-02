
using LabMS.Contracts.Invoice;

namespace LabMS.Validators;

public class InvoiceCreateRequestValidator : AbstractValidator<InvoiceCreateRequest>
{
    public InvoiceCreateRequestValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0")
            .LessThan(1000000).WithMessage("Total amount cannot exceed 1,000,000");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative")
            .LessThan(100).WithMessage("Discount cannot exceed 100%")
            .LessThanOrEqualTo(x => x.TotalAmount).WithMessage("Discount cannot exceed total amount");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
