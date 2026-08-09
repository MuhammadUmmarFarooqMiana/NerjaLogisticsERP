using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> builder)
    {
        builder.Property(f => f.Amount).HasColumnType("decimal(10,2)");
        builder.Property(f => f.Reason).HasMaxLength(5000).IsRequired();
        builder.HasIndex(f => new { f.EmployeeId, f.FineDate });
        builder.HasOne(f => f.Employee).WithMany().HasForeignKey(f => f.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}
