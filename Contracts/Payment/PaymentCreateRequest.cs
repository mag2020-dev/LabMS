namespace LabMS.Contracts.Payment;

public record PaymentCreateRequest(
    Guid InvoiceId,
    decimal Amount,
    string Method,
    string? Reference
);