namespace LabMS.Contracts.Payment;


public record PaymentResponse(
    Guid Id,
    Guid InvoiceId,
    decimal Amount,
    DateTime PaymentDate,
    string Method,
    string? Reference
);
