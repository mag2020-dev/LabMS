namespace LabMS.Contracts.Invoice;

public record InvoiceCreateRequest(
    Guid VisitId,
    decimal TotalAmount,
    decimal Discount,
    string? Notes
);