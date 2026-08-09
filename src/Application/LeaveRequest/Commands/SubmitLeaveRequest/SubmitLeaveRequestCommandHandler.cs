using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.SubmitLeaveRequest;

public class SubmitLeaveRequestCommandHandler : IRequestHandler<SubmitLeaveRequestCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SubmitLeaveRequestCommandHandler> _logger;

    public SubmitLeaveRequestCommandHandler(IApplicationDbContext context, ILogger<SubmitLeaveRequestCommandHandler> logger)
    { _context = context; _logger = logger; }

    public async Task<Guid> Handle(SubmitLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var leave = Domain.Entities.LeaveRequest.Submit(request.EmployeeId, request.StartDate, request.EndDate, request.Reason);
        _context.LeaveRequests.Add(leave);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Leave request submitted: Employee {EmployeeId}, {Start}-{End}", request.EmployeeId, request.StartDate, request.EndDate);
        return leave.Id;
    }
}
