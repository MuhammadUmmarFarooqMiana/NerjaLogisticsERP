namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaryById;

public record GetMonthlySummaryByIdQuery : IRequest<MonthlySummaryDto>
{
    public Guid Id { get; init; }
}
