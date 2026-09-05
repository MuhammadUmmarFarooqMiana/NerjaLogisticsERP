using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.Property(e => e.Amount).HasColumnType("decimal(10,2)");
        builder.Property(e => e.Description).HasMaxLength(5000);
        builder.Property(e => e.Category).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(e => new { e.ExpenseDate, e.Category });
        builder.HasOne(e => e.Platform).WithMany().HasForeignKey(e => e.PlatformId).OnDelete(DeleteBehavior.Restrict);
    }
}
