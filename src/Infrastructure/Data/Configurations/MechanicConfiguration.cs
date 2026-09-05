using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class MechanicConfiguration : IEntityTypeConfiguration<Mechanic>
{
    public void Configure(EntityTypeBuilder<Mechanic> builder)
    {
        builder.Property(m => m.Name).HasMaxLength(150).IsRequired();
        builder.Property(m => m.Phone).HasMaxLength(30);
        builder.Property(m => m.Email).HasMaxLength(256);
        builder.Property(m => m.Specialty).HasMaxLength(200);
        builder.Property(m => m.Address).HasMaxLength(500);
        builder.HasIndex(m => m.Name);
    }
}
