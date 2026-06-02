

namespace LabMS.Entities;

    public class Patient
    {
    public Guid Id { get; set; } = Guid.NewGuid();

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        // Navigation
        public ICollection<Visit> Visits { get; set; } = [];
    }

