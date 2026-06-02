using LabMS.Contracts.Patient;

namespace LabMS.Contracts.Invoice;

public record InvoiceResponse(
    Guid Id,
    Guid VisitId,
    DateTime IssuedAt,
    decimal TotalAmount,
    decimal Discount,
    string Status,
    string? Notes,
    IEnumerable<PaymentResponse> Payments
);