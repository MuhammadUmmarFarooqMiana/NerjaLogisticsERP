using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.MarkMonthlySummaryAsPaid;

public class MarkMonthlySummaryAsPaidCommandHandler : IRequestHandler<MarkMonthlySummaryAsPaidCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public MarkMonthlySummaryAsPaidCommandHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task Handle(MarkMonthlySummaryAsPaidCommand request, CancellationToken cancellationToken)
    {
        var summary = await _context.MonthlySummaries.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(MonthlySummary), request.Id.ToString());

        summary.MarkAsPaid(_currentUser.Id!.Value, request.PaymentReference);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
