using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetPendingApprovalsCount;

public class GetPendingDailyOrderApprovalsCountQueryHandler : IRequestHandler<GetPendingDailyOrderApprovalsCountQuery, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetPendingDailyOrderApprovalsCountQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(GetPendingDailyOrderApprovalsCountQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DailyOrders.Where(o => o.Status == DailyOrderStatus.Closed);

        var isAdministrator = _user.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator)
        {
            var userId = _user.Id!.Value;
            var supervisorEmployeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

            query = query.Where(o => o.Employee.SupervisorId == supervisorEmployeeId);
        }

        return await query.CountAsync(cancellationToken);
    }
}
