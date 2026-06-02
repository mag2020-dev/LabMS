namespace LabMS.Configurations;

public class PaymentPlanInstallmentConfiguration : IEntityTypeConfiguration<PaymentPlanInstallment>
{
    public void Configure(EntityTypeBuilder<PaymentPlanInstallment> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Amount)
            .HasPrecision(18, 2);

        builder.Property(i => i.AmountPaid)
            .HasPrecision(18, 2);

        builder.Property(i => i.IsPaid)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(i => i.PaymentPlan)
            .WithMany(p => p.Installments)
            .HasForeignKey(i => i.PaymentPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Payment)
            .WithMany()
            .HasForeignKey(i => i.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
