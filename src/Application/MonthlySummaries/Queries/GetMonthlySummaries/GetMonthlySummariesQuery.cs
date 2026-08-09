using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaries;

public record GetMonthlySummariesQuery : IRequest<List<MonthlySummaryDto>>
{
    public Guid? EmployeeId { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public MonthlySummaryStatus? Status { get; init; }
}
