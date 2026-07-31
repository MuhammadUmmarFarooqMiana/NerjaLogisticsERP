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

    public DbSet<TodoList> TodoLists => Set<TodoList>();
    
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Platform> Platforms => Set<Platform>();
    
    public DbSet<Employee> Employees => Set<Employee>();
    
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    
    public DbSet<VehicleAllocationHistory> VehicleAllocationHistories => Set<VehicleAllocationHistory>();
    
    public DbSet<VehicleServiceHistory> VehicleServiceHistories => Set<VehicleServiceHistory>();
    
    public DbSet<VehicleOilChangeHistory> VehicleOilChangeHistories => Set<VehicleOilChangeHistory>();
    
    public DbSet<VehicleTyreReplacementHistory> VehicleTyreReplacementHistories => Set<VehicleTyreReplacementHistory>();
    
    public DbSet<VehicleAccidentHistory> VehicleAccidentHistories => Set<VehicleAccidentHistory>();

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
