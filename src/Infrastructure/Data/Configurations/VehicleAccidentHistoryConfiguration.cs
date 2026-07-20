using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class VehicleAccidentHistoryConfiguration : IEntityTypeConfiguration<VehicleAccidentHistory>
{
    public void Configure(EntityTypeBuilder<VehicleAccidentHistory> builder)
    {
        builder.Property(a => a.Description)
            .HasMaxLength(500);

        builder.Property(a => a.RepairCost)
            .HasColumnType("decimal(10,2)");

        builder.HasOne(a => a.Vehicle)
            .WithMany(v => v.AccidentHistory)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
