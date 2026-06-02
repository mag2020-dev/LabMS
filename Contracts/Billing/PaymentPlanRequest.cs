namespace LabMS.Contracts.Billing;

public record PaymentPlanCreateRequest(
    Guid InvoiceId,
    Guid PatientId,
    decimal DownPayment,
    int NumberOfInstallments,
    PaymentFrequency Frequency,
    DateTime StartDate
);

public record PaymentPlanResponse(
    Guid Id,
    Guid InvoiceId,
    Guid PatientId,
    string PatientName,
    decimal TotalAmount,
    decimal DownPayment,
    decimal InstallmentAmount,
    int NumberOfInstallments,
    int InstallmentsPaid,
    PaymentFrequency Frequency,
    DateTime StartDate,
    DateTime? NextPaymentDate,
    PaymentPlanStatus Status,
    DateTime CreatedAt,
    List<PaymentPlanInstallmentResponse> Installments
);

public record PaymentPlanInstallmentResponse(
    Guid Id,
    int InstallmentNumber,
    decimal Amount,
    DateTime DueDate,
    bool IsPaid,
    DateTime? PaidDate,
    decimal? AmountPaid
);

public record PaymentPlanPaymentRequest(
    Guid InstallmentId,
    decimal Amount,
    string PaymentMethod,
    string? Reference
);
