using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class VehicleTyreReplacementHistoryConfiguration : IEntityTypeConfiguration<VehicleTyreReplacementHistory>
{
    public void Configure(EntityTypeBuilder<VehicleTyreReplacementHistory> builder)
    {
        builder.Property(t => t.Cost)
            .HasColumnType("decimal(10,2)");

        builder.HasOne(t => t.Vehicle)
            .WithMany(v => v.TyreReplacements)
            .HasForeignKey(t => t.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
