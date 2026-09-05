using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
namespace NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;

public class UpsertDailyOrderCommandHandler : IRequestHandler<UpsertDailyOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly ILogger<UpsertDailyOrderCommandHandler> _logger;

    public UpsertDailyOrderCommandHandler(IApplicationDbContext context, IUser user, ILogger<UpsertDailyOrderCommandHandler> logger)
    {
        _context = context;
        _user = user;
        _logger = logger;
    }

    public async Task<Guid> Handle(UpsertDailyOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var employeeId = await _context.Employees
            .Where(e => e.UserId == userId)
            .Select(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (employeeId == Guid.Empty)
            throw new NotFoundException(nameof(Employee), userId.ToString());

        var today = DailyOrderClock.Today();

        var order = await _context.DailyOrders.FirstOrDefaultAsync(
            o => o.EmployeeId == employeeId && o.OrderDate == today, cancellationToken);

        if (order is null)
        {
            order = DailyOrder.StartForDate(employeeId, today);
            _context.DailyOrders.Add(order);
            _logger.LogInformation("Daily order started for Employee {EmployeeId} on {Date}", employeeId, today);
        }

        order.UpdateCount(request.CompletedOrders);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Daily order updated: Employee {EmployeeId}, {Count} orders on {Date}",
            employeeId, request.CompletedOrders, today);

        return order.Id;
    }
}
