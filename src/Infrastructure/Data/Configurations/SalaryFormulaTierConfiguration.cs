using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class SalaryFormulaTierConfiguration : IEntityTypeConfiguration<SalaryFormulaTier>
{
    public void Configure(EntityTypeBuilder<SalaryFormulaTier> builder)
    {
        builder.Property(t => t.RateType).HasConversion<string>().HasMaxLength(15);
        builder.Property(t => t.Rate).HasColumnType("decimal(10,2)");
    }
}
