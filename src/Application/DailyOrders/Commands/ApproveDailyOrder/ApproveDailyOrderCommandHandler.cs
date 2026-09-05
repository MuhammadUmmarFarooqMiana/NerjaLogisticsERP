using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.ApproveDailyOrder;

public class ApproveDailyOrderCommandHandler : IRequestHandler<ApproveDailyOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public ApproveDailyOrderCommandHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task Handle(ApproveDailyOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.DailyOrders.FindAsync(new object[] { request.DailyOrderId }, cancellationToken)
            ?? throw new NotFoundException(nameof(DailyOrder), request.DailyOrderId.ToString());

        // A plain Supervisor may only review their own reports' orders —
        // Administrator is unrestricted.
        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator)
        {
            var userId = _currentUser.Id!.Value;
            var isOwnReport = await _context.Employees.AnyAsync(
                e => e.Id == order.EmployeeId && e.Supervisor != null && e.Supervisor.UserId == userId,
                cancellationToken);
            if (!isOwnReport) throw new ForbiddenAccessException();
        }

        order.Approve(_currentUser.Id!.Value);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
