using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.LeaveRequest.Queries;

namespace NerjaLogisticsERP.Application.LeaveRequest.Queries.GetMyLeaveRequests;

[Authorize]
public record GetMyLeaveRequestsQuery : IRequest<PaginatedList<LeaveRequestDto>>
{
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
