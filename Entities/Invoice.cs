

namespace LabMS.Entities;

public class Invoice
    {
        public Guid Id { get; set; }

        public Guid VisitId { get; set; }
        public Visit Visit { get; set; } = default!;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Partial
        public string? Notes { get; set; }

        // Navigation
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

