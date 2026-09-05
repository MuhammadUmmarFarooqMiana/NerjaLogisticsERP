using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.DailyOrders.Queries;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetDailyOrderHistory;

public class GetDailyOrderHistoryQueryHandler : IRequestHandler<GetDailyOrderHistoryQuery, PaginatedList<DailyOrderListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetDailyOrderHistoryQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<DailyOrderListItemDto>> Handle(GetDailyOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate <= request.EndDate ? request.StartDate : request.EndDate;
        var endDate = request.StartDate <= request.EndDate ? request.EndDate : request.StartDate;

        var isSupervisor = _user.Roles?.Contains(Roles.Supervisor) ?? false;
        var isAdministrator = _user.Roles?.Contains(Roles.Administrator) ?? false;
        var userId = _user.Id!.Value;

        // "History" means anything past the in-progress Open state — Closed
        // (awaiting review), Approved, and Rejected all belong here so
        // reviewers can see the full trail, not just what's been approved.
        var ordersQuery = _context.DailyOrders
            .Where(o => o.Status != DailyOrderStatus.Open && o.OrderDate >= startDate && o.OrderDate <= endDate);

        if (isAdministrator)
        {
            // Every employee's completed history.
        }
        else if (isSupervisor)
        {
            var supervisorEmployeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

            ordersQuery = ordersQuery.Where(o => o.Employee.SupervisorId == supervisorEmployeeId);
        }
        else
        {
            ordersQuery = ordersQuery.Where(o => o.Employee.UserId == userId);
        }

        return await ordersQuery
            .OrderByDescending(o => o.OrderDate)
            .ThenBy(o => o.Employee.FullName)
            .Select(o => new DailyOrderListItemDto
            {
                Id = o.Id,
                EmployeeId = o.EmployeeId,
                EmployeeName = o.Employee.FullName,
                OrderDate = o.OrderDate,
                CompletedOrders = o.CompletedOrders,
                Status = o.Status.ToString(),
                ClosedAt = o.ClosedAt,
                ReviewNote = o.ReviewNote
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
