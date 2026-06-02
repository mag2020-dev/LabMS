namespace LabMS.Contracts.Invoice;

public record InvoiceUpdateRequest(
    decimal TotalAmount,
    decimal Discount,
    string Status,
    string? Notes
);