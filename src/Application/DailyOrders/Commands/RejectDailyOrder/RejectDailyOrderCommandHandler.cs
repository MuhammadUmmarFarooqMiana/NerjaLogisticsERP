using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.RejectDailyOrder;

public class RejectDailyOrderCommandHandler : IRequestHandler<RejectDailyOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public RejectDailyOrderCommandHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task Handle(RejectDailyOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.DailyOrders.FindAsync(new object[] { request.DailyOrderId }, cancellationToken)
            ?? throw new NotFoundException(nameof(DailyOrder), request.DailyOrderId.ToString());

        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator)
        {
            var userId = _currentUser.Id!.Value;
            var isOwnReport = await _context.Employees.AnyAsync(
                e => e.Id == order.EmployeeId && e.Supervisor != null && e.Supervisor.UserId == userId,
                cancellationToken);
            if (!isOwnReport) throw new ForbiddenAccessException();
        }

        order.Reject(_currentUser.Id!.Value, request.Reason);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
