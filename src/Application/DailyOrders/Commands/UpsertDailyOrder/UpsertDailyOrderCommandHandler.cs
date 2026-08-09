using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
namespace NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;

public class UpsertDailyOrderCommandHandler : IRequestHandler<UpsertDailyOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpsertDailyOrderCommandHandler> _logger;
    private static readonly TimeZoneInfo KsaZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");

    public UpsertDailyOrderCommandHandler(IApplicationDbContext context, ILogger<UpsertDailyOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> Handle(UpsertDailyOrderCommand request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, KsaZone).Date);

        var order = await _context.DailyOrders.FirstOrDefaultAsync(
            o => o.EmployeeId == request.EmployeeId && o.OrderDate == today, cancellationToken);

        if (order is null)
        {
            order = DailyOrder.StartForDate(request.EmployeeId, today);
            _context.DailyOrders.Add(order);
            _logger.LogInformation("Daily order started for Employee {EmployeeId} on {Date}", request.EmployeeId, today);
        }

        order.UpdateCount(request.CompletedOrders);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Daily order updated: Employee {EmployeeId}, {Count} orders on {Date}",
            request.EmployeeId, request.CompletedOrders, today);

        return order.Id;
    }
}
