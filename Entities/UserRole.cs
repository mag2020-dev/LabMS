

namespace LabMS.Entities;

// Join table for many-to-many between User and Role
public class UserRole
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = default!;
    }

