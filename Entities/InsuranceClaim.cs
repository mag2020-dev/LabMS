namespace LabMS.Entities;

public class InsuranceClaim
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ClaimNumber { get; set; } = default!;
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public string InsuranceProvider { get; set; } = default!;
    public string PolicyNumber { get; set; } = default!;
    public string? GroupNumber { get; set; }

    public decimal ClaimAmount { get; set; }
    public decimal ApprovedAmount { get; set; } = 0;
    public decimal PatientResponsibility { get; set; } = 0;

    public ClaimStatus Status { get; set; } = ClaimStatus.Draft;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }

    public Guid SubmittedByUserId { get; set; }
    public User SubmittedBy { get; set; } = default!;
}

public enum ClaimStatus
{
    Draft,
    Submitted,
    UnderReview,
    Approved,
    PartiallyApproved,
    Rejected,
    Paid
}
