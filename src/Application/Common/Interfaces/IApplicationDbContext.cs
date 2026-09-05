using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Platform> Platforms { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<VehicleAllocationHistory> VehicleAllocationHistories { get; }
    DbSet<VehicleServiceHistory> VehicleServiceHistories { get; }
    DbSet<VehicleOilChangeHistory> VehicleOilChangeHistories { get; }
    DbSet<VehicleTyreReplacementHistory> VehicleTyreReplacementHistories { get; }
    DbSet<VehicleAccidentHistory> VehicleAccidentHistories { get; }
    DbSet<DailyOrder> DailyOrders { get; }
    DbSet<Fine> Fines { get; }
    DbSet<Advance> Advances { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<Domain.Entities.LeaveRequest> LeaveRequests { get; }
    DbSet<Domain.Entities.EmployeeDocument> EmployeeDocuments { get; }
    DbSet<CompanyDocument> CompanyDocuments { get; }
    DbSet<SalaryFormula> SalaryFormulas { get; }
    DbSet<SalaryFormulaTier> SalaryFormulaTiers { get; }
    DbSet<MonthlySummary> MonthlySummaries { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<StockIn> StockIns { get; }
    DbSet<StockOut> StockOuts { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<Mechanic> Mechanics { get; }
    DbSet<CompanyDocumentFolder> CompanyDocumentFolders { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
