

namespace LabMS.Entities;

public class TestResult
    {
        public Guid Id { get; set; }

        public Guid TestOrderId { get; set; }
        public TestOrder TestOrder { get; set; } = default!;
        public string TestName { get; set; } = default!;

        public string ResultValue { get; set; } = default!;
        public string? Unit { get; set; }
        public string? ReferenceRange { get; set; }
        public string? Notes { get; set; }

        public DateTime ResultDate { get; set; } = DateTime.UtcNow;

        public Guid? VerifiedById { get; set; }
        public User? VerifiedBy { get; set; }
    }

