using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.SubmitLeaveRequest;

public class SubmitLeaveRequestCommandHandler : IRequestHandler<SubmitLeaveRequestCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly ILogger<SubmitLeaveRequestCommandHandler> _logger;

    public SubmitLeaveRequestCommandHandler(IApplicationDbContext context, IUser user, ILogger<SubmitLeaveRequestCommandHandler> logger)
    { _context = context; _user = user; _logger = logger; }

    public async Task<Guid> Handle(SubmitLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var employeeId = await _context.Employees
            .Where(e => e.UserId == userId)
            .Select(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (employeeId == Guid.Empty) throw new NotFoundException(nameof(Employee), userId.ToString());

        var leave = Domain.Entities.LeaveRequest.Submit(employeeId, request.StartDate, request.EndDate, request.Reason);
        _context.LeaveRequests.Add(leave);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Leave request submitted: Employee {EmployeeId}, {Start}-{End}", employeeId, request.StartDate, request.EndDate);
        return leave.Id;
    }
}
