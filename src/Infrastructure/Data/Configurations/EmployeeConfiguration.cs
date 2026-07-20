using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Infrastructure.Identity;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Employee>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.IqamaNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(e => e.IqamaNumber)
            .IsUnique();

        builder.Property(e => e.PlatformIdNumber)
            .HasMaxLength(50);

        // Store enums as readable strings, not raw ints — future-you (or a DBA
        // running ad-hoc queries) will thank you when AccountStatus reads
        // "Suspended" in a query result instead of "2".
        builder.Property(e => e.AccountStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.PerformanceStatus)
            .HasConversion<string>()
            .HasMaxLength(10);

        // Platform relationship — optional (Administrator/SoftwareEngineer employees
        // could theoretically have none, though in practice Employee is only ever
        // created for Supervisor/Accountant/Rider per our earlier decision).
        builder.HasOne(e => e.Platform)
            .WithMany()
            .HasForeignKey(e => e.PlatformId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Vehicle)
            .WithMany()
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing Supervisor — MUST be Restrict, not the EF default,
        // or Postgres will reject the migration outright due to ambiguous
        // multiple cascade paths on a self-referencing FK.
        builder.HasOne(e => e.Supervisor)
            .WithMany()
            .HasForeignKey(e => e.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);
    }


}
