using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Fines.Commands.CreateFine;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record CreateFineCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public decimal Amount { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateOnly FineDate { get; init; }
}
