using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.SuspendEmployee;

public class SuspendEmployeeCommandHandler : IRequestHandler<SuspendEmployeeCommand>
{
    private readonly IApplicationDbContext _context;
    public SuspendEmployeeCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(SuspendEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.Id.ToString());

        employee.Suspend();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
