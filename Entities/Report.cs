

namespace LabMS.Entities;

public class Report
    {
        public Guid Id { get; set; }

        public Guid VisitId { get; set; }
        public Visit Visit { get; set; } = default!;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string FilePath { get; set; } = default!; // or store binary/blob elsewhere

        // optional: who generated it
        public Guid? GeneratedById { get; set; }
        public User? GeneratedBy { get; set; }
    }

