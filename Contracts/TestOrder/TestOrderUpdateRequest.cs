namespace LabMS.Contracts.TestOrder;

public record TestOrderUpdateRequest(
    string? LabTestName,
    string Status
);