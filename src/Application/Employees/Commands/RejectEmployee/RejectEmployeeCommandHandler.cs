using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.RejectEmployee;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public class RejectEmployeeCommandHandler : IRequestHandler<RejectEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public RejectEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RejectEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        employee.Reject(request.Reason);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
