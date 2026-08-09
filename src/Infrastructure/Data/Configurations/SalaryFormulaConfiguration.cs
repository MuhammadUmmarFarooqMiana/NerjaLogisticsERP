using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class SalaryFormulaConfiguration : IEntityTypeConfiguration<SalaryFormula>
{
    public void Configure(EntityTypeBuilder<SalaryFormula> builder)
    {
        builder.Property(f => f.FormulaType).HasConversion<string>().HasMaxLength(20);
        builder.Property(f => f.FixedMonthlyAmount).HasColumnType("decimal(10,2)");
        builder.HasIndex(f => new { f.PlatformId, f.EffectiveFrom, f.EffectiveTo });
        builder.HasOne(f => f.Platform).WithMany().HasForeignKey(f => f.PlatformId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(f => f.Tiers).WithOne().HasForeignKey(t => t.SalaryFormulaId).OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_SalaryFormulas_EffectiveTo_After_EffectiveFrom",
            "\"EffectiveTo\" IS NULL OR \"EffectiveTo\" > \"EffectiveFrom\""));
    }
}
