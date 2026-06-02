

namespace LabMS.Entities;

public class Payment
    {
        public Guid Id { get; set; }

        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = default!;

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string Method { get; set; } = default!; // Cash, Card, Insurance, etc.
        public string? Reference { get; set; } // transaction id, cheque no, etc.
    }

