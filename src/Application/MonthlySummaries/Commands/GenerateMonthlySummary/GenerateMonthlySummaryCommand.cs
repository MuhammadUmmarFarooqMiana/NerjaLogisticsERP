using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GenerateMonthlySummaryCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
}
