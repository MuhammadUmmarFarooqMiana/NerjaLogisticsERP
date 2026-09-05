using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Suppliers.Commands.UpdateSupplier;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record UpdateSupplierCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
}
