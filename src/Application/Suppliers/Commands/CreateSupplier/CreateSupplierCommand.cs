using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Suppliers.Commands.CreateSupplier;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record CreateSupplierCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
}
