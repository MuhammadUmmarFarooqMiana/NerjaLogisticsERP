using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.Property(v => v.RegistrationNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(v => v.RegistrationNumber)
            .IsUnique();

        builder.Property(v => v.VehicleType)
            .HasConversion<string>()
            .HasMaxLength(20);

        // As Navigation collections are configured from the "many" side in each
        // history entity's own config below — nothing else needed here.
    }
}
