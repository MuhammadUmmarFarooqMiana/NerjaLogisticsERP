using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;

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

        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator)
        {
            var userId = _currentUser.Id!.Value;
            var isOwnReport = await _context.Employees.AnyAsync(
                e => e.Id == leave.EmployeeId && e.Supervisor != null && e.Supervisor.UserId == userId,
                cancellationToken);
            if (!isOwnReport) throw new ForbiddenAccessException();
        }

        leave.Reject(_currentUser.Id!.Value, request.Reason);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
