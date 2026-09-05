using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.ApproveLeaveRequest;

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

        // A plain Supervisor may only review their own reports' requests —
        // Administrator is unrestricted.
        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator)
        {
            var userId = _currentUser.Id!.Value;
            var isOwnReport = await _context.Employees.AnyAsync(
                e => e.Id == leave.EmployeeId && e.Supervisor != null && e.Supervisor.UserId == userId,
                cancellationToken);
            if (!isOwnReport) throw new ForbiddenAccessException();
        }

        leave.Approve(_currentUser.Id!.Value);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
