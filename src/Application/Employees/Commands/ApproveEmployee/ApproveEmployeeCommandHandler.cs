using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public class ApproveEmployeeCommandHandler : IRequestHandler<ApproveEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public ApproveEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ApproveEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        employee.Approve(request.JoiningDate);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
