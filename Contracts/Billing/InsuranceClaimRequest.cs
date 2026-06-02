namespace LabMS.Contracts.Billing;

public record InsuranceClaimCreateRequest(
    Guid InvoiceId,
    Guid PatientId,
    string InsuranceProvider,
    string PolicyNumber,
    string? GroupNumber,
    decimal ClaimAmount,
    string? Notes
);

public record InsuranceClaimUpdateRequest(
    ClaimStatus? Status,
    decimal? ApprovedAmount,
    decimal? PatientResponsibility,
    string? RejectionReason,
    string? Notes
);

public record InsuranceClaimResponse(
    Guid Id,
    string ClaimNumber,
    Guid InvoiceId,
    Guid PatientId,
    string PatientName,
    string InsuranceProvider,
    string PolicyNumber,
    string? GroupNumber,
    decimal ClaimAmount,
    decimal ApprovedAmount,
    decimal PatientResponsibility,
    ClaimStatus Status,
    DateTime SubmittedAt,
    DateTime? ProcessedAt,
    DateTime? PaidAt,
    string? RejectionReason,
    string? Notes,
    string SubmittedByName
);
