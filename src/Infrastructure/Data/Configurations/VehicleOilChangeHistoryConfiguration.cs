using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class VehicleOilChangeHistoryConfiguration : IEntityTypeConfiguration<VehicleOilChangeHistory>
{
    public void Configure(EntityTypeBuilder<VehicleOilChangeHistory> builder)
    {
        builder.Property(o => o.Cost)
            .HasColumnType("decimal(10,2)");

        builder.HasOne(o => o.Vehicle)
            .WithMany(v => v.OilChanges)
            .HasForeignKey(o => o.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
