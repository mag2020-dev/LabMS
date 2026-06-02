
using LabMS.Contracts.Invoice;

namespace LabMS.Validators;

public class InvoiceUpdateRequestValidator : AbstractValidator<InvoiceUpdateRequest>
{
    public InvoiceUpdateRequestValidator()
    {
        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0")
            .LessThan(1000000).WithMessage("Total amount cannot exceed 1,000,000");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative")
            .LessThan(100).WithMessage("Discount cannot exceed 100%");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => status is "Unpaid" or "Paid" or "Partial")
            .WithMessage("Status must be one of: Unpaid, Paid, Partial");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
