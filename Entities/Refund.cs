namespace LabMS.Entities;

public class Refund
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string RefundNumber { get; set; } = default!;
    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } = default!;

    public decimal Amount { get; set; }
    public string Reason { get; set; } = default!;
    public RefundMethod Method { get; set; }

    public RefundStatus Status { get; set; } = RefundStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }

    public Guid RequestedByUserId { get; set; }
    public User RequestedBy { get; set; } = default!;

    public Guid? ApprovedByUserId { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public string? Notes { get; set; }
}

public enum RefundMethod
{
    Cash,
    BankTransfer,
    CreditCard,
    OriginalPaymentMethod
}

public enum RefundStatus
{
    Pending,
    Approved,
    Rejected,
    Processed,
    Completed
}
