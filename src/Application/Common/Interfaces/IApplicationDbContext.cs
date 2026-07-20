using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }

    DbSet<Platform> Platforms { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<VehicleAllocationHistory> VehicleAllocationHistories { get; }
    DbSet<VehicleServiceHistory> VehicleServiceHistories { get; }
    DbSet<VehicleOilChangeHistory> VehicleOilChangeHistories { get; }
    DbSet<VehicleTyreReplacementHistory> VehicleTyreReplacementHistories { get; }
    DbSet<VehicleAccidentHistory> VehicleAccidentHistories { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
