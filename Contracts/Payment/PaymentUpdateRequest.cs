namespace LabMS.Contracts.Payment;

public record PaymentUpdateRequest(
    decimal Amount,
    string Method,
    string? Reference
);