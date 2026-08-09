using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.VerifyMonthlySummary;

[Authorize(Roles = Roles.Accountant)]
public class VerifyMonthlySummaryCommandHandler : IRequestHandler<VerifyMonthlySummaryCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public VerifyMonthlySummaryCommandHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task Handle(VerifyMonthlySummaryCommand request, CancellationToken cancellationToken)
    {
        var summary = await _context.MonthlySummaries.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(MonthlySummary), request.Id.ToString());

        summary.Verify(_currentUser.Id!.Value);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
