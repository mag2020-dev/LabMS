

namespace LabMS.Entities;

public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!; // Admin, Technician, Receptionist, LabManager

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
    }

