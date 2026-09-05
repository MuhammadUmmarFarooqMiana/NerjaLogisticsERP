using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.VerifyMonthlySummary;

[Authorize(Roles = Roles.Accountant)]
public record VerifyMonthlySummaryCommand : IRequest { public Guid Id { get; init; } }

