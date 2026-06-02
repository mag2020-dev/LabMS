

namespace LabMS.Entities;

public class Doctor
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Specialty { get; set; }
        public string? PhoneNumber { get; set; }


    // Navigation
    public ICollection<Visit> Visits { get; set; } = [];
    }

