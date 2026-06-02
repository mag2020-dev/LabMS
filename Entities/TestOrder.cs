

namespace LabMS.Entities;

public class TestOrder
    {
        public Guid Id { get; set; }

        public Guid VisitId { get; set; }
        public Visit Visit { get; set; } = default!;

        public Guid LabTestId { get; set; }
        public LabTest LabTest { get; set; } = default!;

        public string Status { get; set; } = "Ordered"; // Ordered, InProgress, Completed, Cancelled
        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public TestResult? TestResult { get; set; }
    }

