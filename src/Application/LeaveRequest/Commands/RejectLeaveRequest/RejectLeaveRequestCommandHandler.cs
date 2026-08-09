using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public class RejectLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public RejectLeaveRequestCommandHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leave = await _context.LeaveRequests.FindAsync(new object[] { request.LeaveRequestId }, cancellationToken)
            ?? throw new NotFoundException(nameof(LeaveRequest), request.LeaveRequestId.ToString());

        leave.Reject(_currentUser.Id!.Value, request.Reason);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
