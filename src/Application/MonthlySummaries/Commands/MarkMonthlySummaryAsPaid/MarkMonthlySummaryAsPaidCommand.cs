using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.MarkMonthlySummaryAsPaid;

[Authorize(Roles = Roles.Accountant)]
public record MarkMonthlySummaryAsPaidCommand : IRequest
{
    public Guid Id { get; init; }
    public string? PaymentReference { get; init; }
}
