using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Mechanics.Commands.UpdateMechanic;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record UpdateMechanicCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Specialty { get; init; }
    public string? Address { get; init; }
}
