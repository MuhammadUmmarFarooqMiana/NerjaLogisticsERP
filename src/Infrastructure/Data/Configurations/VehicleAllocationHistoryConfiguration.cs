using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class VehicleAllocationHistoryConfiguration : IEntityTypeConfiguration<VehicleAllocationHistory>
{
    public void Configure(EntityTypeBuilder<VehicleAllocationHistory> builder)
    {
        builder.HasOne(a => a.Vehicle)
            .WithMany(v => v.Allocations)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict, not Cascade: deleting an Employee should never silently
        // wipe out the vehicle allocation history tied to them — that's an
        // audit trail. Reassign/close out allocations explicitly instead.
        builder.HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
