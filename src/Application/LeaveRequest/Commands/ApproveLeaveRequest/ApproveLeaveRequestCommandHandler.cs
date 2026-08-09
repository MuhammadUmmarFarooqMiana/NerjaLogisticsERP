using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.ApproveLeaveRequest;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public class ApproveLeaveRequestCommandHandler : IRequestHandler<ApproveLeaveRequestCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public ApproveLeaveRequestCommandHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leave = await _context.LeaveRequests.FindAsync(new object[] { request.LeaveRequestId }, cancellationToken)
            ?? throw new NotFoundException(nameof(LeaveRequest), request.LeaveRequestId.ToString());

        leave.Approve(_currentUser.Id!.Value);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
