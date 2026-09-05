using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Salaries.Commands.DeactivateSalaryFormula;

public class DeactivateSalaryFormulaCommandHandler : IRequestHandler<DeactivateSalaryFormulaCommand>
{
    private readonly IApplicationDbContext _context;

    public DeactivateSalaryFormulaCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeactivateSalaryFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = await _context.SalaryFormulas.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(SalaryFormula), request.Id.ToString());

        formula.Close(DateOnly.FromDateTime(DateTime.UtcNow));
        await _context.SaveChangesAsync(cancellationToken);
    }
}
