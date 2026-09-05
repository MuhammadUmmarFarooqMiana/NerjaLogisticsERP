using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Common;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Infrastructure.Identity;

namespace NerjaLogisticsERP.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Platform> Platforms => Set<Platform>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public DbSet<VehicleAllocationHistory> VehicleAllocationHistories => Set<VehicleAllocationHistory>();

    public DbSet<VehicleServiceHistory> VehicleServiceHistories => Set<VehicleServiceHistory>();

    public DbSet<VehicleOilChangeHistory> VehicleOilChangeHistories => Set<VehicleOilChangeHistory>();

    public DbSet<VehicleTyreReplacementHistory> VehicleTyreReplacementHistories => Set<VehicleTyreReplacementHistory>();

    public DbSet<VehicleAccidentHistory> VehicleAccidentHistories => Set<VehicleAccidentHistory>();
    public DbSet<DailyOrder> DailyOrders => Set<DailyOrder>();
    public DbSet<Fine> Fines => Set<Fine>();
    public DbSet<Advance> Advances => Set<Advance>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
    public DbSet<CompanyDocument> CompanyDocuments => Set<CompanyDocument>();
    public DbSet<CompanyDocumentFolder> CompanyDocumentFolders => Set<CompanyDocumentFolder>();
    public DbSet<SalaryFormula> SalaryFormulas => Set<SalaryFormula>();
    public DbSet<SalaryFormulaTier> SalaryFormulaTiers => Set<SalaryFormulaTier>();
    public DbSet<MonthlySummary> MonthlySummaries => Set<MonthlySummary>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<StockIn> StockIns => Set<StockIn>();
    public DbSet<StockOut> StockOuts => Set<StockOut>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
