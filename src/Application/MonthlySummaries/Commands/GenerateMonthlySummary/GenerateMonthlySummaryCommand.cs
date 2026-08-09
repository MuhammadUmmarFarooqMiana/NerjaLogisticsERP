namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;

public record GenerateMonthlySummaryCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
}
