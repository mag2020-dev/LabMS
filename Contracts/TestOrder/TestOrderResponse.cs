namespace LabMS.Contracts.TestOrder;

public record TestOrderResponse(
    Guid Id,
    Guid VisitId,
    DateTime VisitDate,
    string PatientName,
    string LabTestName,
    string Status,
    DateTime OrderedAt
);