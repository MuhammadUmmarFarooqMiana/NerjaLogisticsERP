using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class DailyOrderConfiguration : IEntityTypeConfiguration<DailyOrder>
{
    public void Configure(EntityTypeBuilder<DailyOrder> builder)
    {
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(10);

        builder.HasIndex(o => new { o.EmployeeId, o.OrderDate }).IsUnique();

        builder.HasOne(o => o.Employee)
            .WithMany()
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
