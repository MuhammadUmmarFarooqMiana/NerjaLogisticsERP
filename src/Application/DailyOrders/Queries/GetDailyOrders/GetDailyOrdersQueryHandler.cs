using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.DailyOrders.Queries;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetDailyOrders;

public class GetDailyOrdersQueryHandler : IRequestHandler<GetDailyOrdersQuery, PaginatedList<DailyOrderListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetDailyOrdersQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<DailyOrderListItemDto>> Handle(GetDailyOrdersQuery request, CancellationToken cancellationToken)
    {
        var date = request.Date ?? DailyOrderClock.Today();
        var isSupervisor = _user.Roles?.Contains(Roles.Supervisor) ?? false;
        var isAdministrator = _user.Roles?.Contains(Roles.Administrator) ?? false;

        var employeesQuery = _context.Employees.Where(e => e.AccountStatus == AccountStatus.Active);

        // Administrator sees every team; a plain Supervisor is scoped to their own reports.
        if (isSupervisor && !isAdministrator)
        {
            var userId = _user.Id!.Value;
            var supervisorEmployeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

            employeesQuery = employeesQuery.Where(e => e.SupervisorId == supervisorEmployeeId);
        }

        // Left join so riders who haven't logged anything yet today still show up (as Open/0).
        // Ordering happens once, below, after ClosedAt is available on the projected DTO.
        var rows = await employeesQuery
            .GroupJoin(
                _context.DailyOrders.Where(o => o.OrderDate == date),
                e => e.Id,
                o => o.EmployeeId,
                (e, orders) => new { Employee = e, Order = orders.FirstOrDefault() })
            .ToListAsync(cancellationToken);

        // A rider on approved leave has no order to log — without this they'd be
        // indistinguishable from someone who simply hasn't logged in yet.
        var onLeaveEmployeeIds = (await _context.LeaveRequests
            .Where(l => l.Status == LeaveStatus.Approved && l.StartDate <= date && date <= l.EndDate)
            .Select(l => l.EmployeeId)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        var items = rows
            .Select(x => new DailyOrderListItemDto
            {
                Id = x.Order?.Id,
                EmployeeId = x.Employee.Id,
                EmployeeName = x.Employee.FullName,
                OrderDate = date,
                CompletedOrders = x.Order != null ? x.Order.CompletedOrders : 0,
                Status = x.Order != null
                    ? x.Order.Status.ToString()
                    : onLeaveEmployeeIds.Contains(x.Employee.Id) ? "OnLeave" : DailyOrderStatus.Open.ToString(),
                ClosedAt = x.Order != null ? x.Order.ClosedAt : null,
                ReviewNote = x.Order?.ReviewNote
            })
            // Most recently closed first, so a Supervisor/Admin reviewing today's
            // roster sees the newest awaiting-approval orders at the top. Rows with
            // no ClosedAt (Open/OnLeave/hasn't logged anything) sort after every
            // closed row — Nullable<DateTimeOffset>'s default comparer treats null
            // as less than any value, so descending naturally pushes them to the
            // end — and fall back to employee name among themselves for a stable,
            // readable order.
            .OrderByDescending(i => i.ClosedAt)
            .ThenBy(i => i.EmployeeName)
            .ToList();

        return PaginatedList<DailyOrderListItemDto>.Create(items, request.PageNumber, request.PageSize);
    }
}
