using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class VehicleServiceHistoryConfiguration : IEntityTypeConfiguration<VehicleServiceHistory>
{
    public void Configure(EntityTypeBuilder<VehicleServiceHistory> builder)
    {
        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.Cost)
            .HasColumnType("decimal(10,2)");

        builder.HasOne(s => s.Vehicle)
            .WithMany(v => v.ServiceHistory)
            .HasForeignKey(s => s.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
