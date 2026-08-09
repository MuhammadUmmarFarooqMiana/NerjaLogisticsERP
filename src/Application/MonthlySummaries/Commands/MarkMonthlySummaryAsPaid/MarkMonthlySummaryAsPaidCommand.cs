namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.MarkMonthlySummaryAsPaid;

public record MarkMonthlySummaryAsPaidCommand : IRequest
{
    public Guid Id { get; init; }
    public string? PaymentReference { get; init; }
}
