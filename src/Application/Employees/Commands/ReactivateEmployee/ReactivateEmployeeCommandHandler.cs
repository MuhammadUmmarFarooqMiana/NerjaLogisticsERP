using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.ReactivateEmployee;

public class ReactivateEmployeeCommandHandler : IRequestHandler<ReactivateEmployeeCommand>
{
    private readonly IApplicationDbContext _context;
    public ReactivateEmployeeCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ReactivateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.Id.ToString());

        employee.Reactivate();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
