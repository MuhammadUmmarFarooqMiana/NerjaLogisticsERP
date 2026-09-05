using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.LeaveRequest.Queries;

namespace NerjaLogisticsERP.Application.LeaveRequest.Queries.GetMyLeaveRequests;

public class GetMyLeaveRequestsQueryHandler : IRequestHandler<GetMyLeaveRequestsQuery, PaginatedList<LeaveRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyLeaveRequestsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<LeaveRequestDto>> Handle(GetMyLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;

        return await _context.LeaveRequests
            .Where(l => l.Employee.UserId == userId)
            .OrderByDescending(l => l.StartDate)
            .Select(l => new LeaveRequestDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.FullName,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status.ToString(),
                ReviewedByName = l.ReviewedBy != null
                    ? _context.Employees.Where(e => e.UserId == l.ReviewedBy).Select(e => e.FullName).FirstOrDefault()
                    : null,
                ReviewedAt = l.ReviewedAt,
                RejectionReason = l.RejectionReason
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
