namespace LabMS.Contracts.TestOrder;



public record TestOrderCreateRequest(
    Guid VisitId,
    string LabTestName
);