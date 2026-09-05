using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaryById;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetMonthlySummaryByIdQuery : IRequest<MonthlySummaryDto>
{
    public Guid Id { get; init; }
}
