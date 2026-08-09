namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.VerifyMonthlySummary;

public record VerifyMonthlySummaryCommand : IRequest { public Guid Id { get; init; } }

