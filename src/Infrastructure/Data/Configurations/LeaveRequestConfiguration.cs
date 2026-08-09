using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.Property(l => l.Status).HasConversion<string>().HasMaxLength(15);
        builder.Property(l => l.Reason).HasMaxLength(5000);
        builder.Property(l => l.RejectionReason).HasMaxLength(5000);
        builder.HasIndex(l => new { l.EmployeeId, l.Status });
        builder.HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}
