using LabMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabMS.Configurations
{
    public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
    {
        public void Configure(EntityTypeBuilder<TestResult> builder)
        {
            builder.HasKey(tr => tr.Id);

            builder.Property(tr => tr.TestName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(tr => tr.ResultValue)
                   .IsRequired()
                   .HasMaxLength(200); 
            builder.Property(tr => tr.Unit)
                   .HasMaxLength(50);

            builder.Property(tr => tr.ReferenceRange)
                   .HasMaxLength(250);

            builder.Property(tr => tr.Notes)
                   .HasMaxLength(1000);

            builder.Property(tr => tr.ResultDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(tr => tr.TestOrder)
                   .WithOne(to => to.TestResult)
                   .HasForeignKey<TestResult>(tr => tr.TestOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tr => tr.VerifiedBy)
                   .WithMany()
                   .HasForeignKey(tr => tr.VerifiedById)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(tr => tr.TestOrderId).IsUnique(); // one result per order
            builder.HasIndex(tr => tr.VerifiedById);
        }
    }
}
