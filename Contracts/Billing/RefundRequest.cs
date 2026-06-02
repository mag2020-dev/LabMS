namespace LabMS.Contracts.Billing;

public record RefundCreateRequest(
    Guid PaymentId,
    decimal Amount,
    string Reason,
    RefundMethod Method,
    string? Notes
);

public record RefundApprovalRequest(
    bool IsApproved,
    string? Notes
);

public record RefundResponse(
    Guid Id,
    string RefundNumber,
    Guid PaymentId,
    decimal Amount,
    string Reason,
    RefundMethod Method,
    RefundStatus Status,
    DateTime RequestedAt,
    DateTime? ProcessedAt,
    string RequestedByName,
    string? ApprovedByName,
    DateTime? ApprovedAt,
    string? Notes
);
