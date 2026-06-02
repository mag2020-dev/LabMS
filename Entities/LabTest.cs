

namespace LabMS.Entities;

public class LabTest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? EstimatedDurationHours { get; set; }

    // Navigation
    public ICollection<TestOrder> TestOrders { get; set; } = [];
    }

