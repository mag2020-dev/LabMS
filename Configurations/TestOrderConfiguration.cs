


namespace LabMS.Configurations;

public class TestOrderConfiguration : IEntityTypeConfiguration<TestOrder>
    {
        public void Configure(EntityTypeBuilder<TestOrder> builder)
        {
            builder.HasKey(to => to.Id);
            builder.Property(to => to.Status).IsRequired().HasMaxLength(50);

            builder.HasOne(to => to.Visit)
                   .WithMany(v => v.TestOrders)
                   .HasForeignKey(to => to.VisitId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(to => to.LabTest)
                   .WithMany(lt => lt.TestOrders)
                   .HasForeignKey(to => to.LabTestId)
                   .OnDelete(DeleteBehavior.Restrict);
        builder.Property(to => to.OrderedAt)
                .IsRequired();
    }
}



       

       
        

      





