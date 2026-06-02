namespace LabMS.Entities;

public class PaymentPlanInstallment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PaymentPlanId { get; set; }
    public PaymentPlan PaymentPlan { get; set; } = default!;

    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }

    public bool IsPaid { get; set; } = false;
    public DateTime? PaidDate { get; set; }
    public decimal? AmountPaid { get; set; }

    public Guid? PaymentId { get; set; }
    public Payment? Payment { get; set; }
}
