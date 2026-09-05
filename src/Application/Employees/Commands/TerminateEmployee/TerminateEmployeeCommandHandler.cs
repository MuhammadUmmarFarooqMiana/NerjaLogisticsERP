using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.TerminateEmployee;

public class TerminateEmployeeCommandHandler : IRequestHandler<TerminateEmployeeCommand>
{
    private readonly IApplicationDbContext _context;
    public TerminateEmployeeCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(TerminateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.Id.ToString());

        employee.Terminate();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
