
using LabMS.Contracts.Payment;

namespace LabMS.Validators;

public class PaymentUpdateRequestValidator : AbstractValidator<PaymentUpdateRequest>
{
    public PaymentUpdateRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThan(1000000).WithMessage("Amount cannot exceed 1,000,000");

        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Payment method is required")
            .MaximumLength(50).WithMessage("Payment method cannot exceed 50 characters")
            .Must(method => method.ToLower() is "cash" or "card" or "bank_transfer" or "check" or "online")
            .WithMessage("Payment method must be one of: Cash, Card, Bank Transfer, Check, Online");

        RuleFor(x => x.Reference)
            .MaximumLength(100).WithMessage("Reference cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Reference));
    }
}
