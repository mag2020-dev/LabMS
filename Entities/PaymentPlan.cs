namespace LabMS.Entities;

public class PaymentPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public decimal TotalAmount { get; set; }
    public decimal DownPayment { get; set; }
    public decimal InstallmentAmount { get; set; }
    public int NumberOfInstallments { get; set; }
    public int InstallmentsPaid { get; set; } = 0;

    public PaymentFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }

    public PaymentPlanStatus Status { get; set; } = PaymentPlanStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CreatedByUserId { get; set; }
    public User CreatedBy { get; set; } = default!;

    // Navigation
    public ICollection<PaymentPlanInstallment> Installments { get; set; } = [];
}

public enum PaymentFrequency
{
    Weekly,
    BiWeekly,
    Monthly,
    Quarterly
}

public enum PaymentPlanStatus
{
    Active,
    Completed,
    Defaulted,
    Cancelled
}
