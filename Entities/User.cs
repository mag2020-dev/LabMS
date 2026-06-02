

namespace LabMS.Entities;

public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;

        // Navigation - many-to-many via UserRole
        public ICollection<UserRole> UserRoles { get; set; } = [];

        // Optional: navigation for verifications/generation
        public ICollection<TestResult> VerifiedResults { get; set; } = [];
        public ICollection<Report> GeneratedReports { get; set; } = [];
    }

