using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class MonthlySummaryConfiguration : IEntityTypeConfiguration<MonthlySummary>
{
    public void Configure(EntityTypeBuilder<MonthlySummary> builder)
    {
        builder.Property(s => s.TotalSalary).HasColumnType("decimal(12,2)");
        builder.Property(s => s.TotalAdvances).HasColumnType("decimal(12,2)");
        builder.Property(s => s.TotalFines).HasColumnType("decimal(12,2)");
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(15);
        builder.Property(s => s.PaymentReference).HasMaxLength(100);

        builder.HasIndex(s => new { s.EmployeeId, s.Year, s.Month }).IsUnique();
        builder.HasOne(s => s.Employee).WithMany().HasForeignKey(s => s.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}
