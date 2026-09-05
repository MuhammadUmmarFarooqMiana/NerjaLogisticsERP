using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Advances.Commands.CreateAdvance;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record CreateAdvanceCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public decimal Amount { get; init; }
    public DateOnly AdvanceDate { get; init; }
    public string? Remarks { get; init; }
}
