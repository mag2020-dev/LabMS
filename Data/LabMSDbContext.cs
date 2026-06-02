
namespace LabMS.Data;

public class LabMSDbContext(DbContextOptions<LabMSDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<TestOrder> TestOrders { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<LabTest> LabTests { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<TestPackage> TestPackages { get; set; }
    public DbSet<TestPackageItem> TestPackageItems { get; set; }
    public DbSet<PackageSubscription> PackageSubscriptions { get; set; }
    public DbSet<Sample> Samples { get; set; }
    public DbSet<SampleTracking> SampleTrackings { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<InventoryAlert> InventoryAlerts { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }


    // Billing & Financials
    public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
    public DbSet<PaymentPlan> PaymentPlans { get; set; }
    public DbSet<PaymentPlanInstallment> PaymentPlanInstallments { get; set; }
    public DbSet<DiscountRule> DiscountRules { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<TaxConfiguration> TaxConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations from the Persistence/Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LabMSDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
        // --- Seed Roles ---
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin" },
            new Role { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Receptionist" },
            new Role { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Technician" },
            new Role { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "LabManager" }
        );
    }
}


