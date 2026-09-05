using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Suppliers.Commands.DeleteSupplier;

[Authorize(Roles = Roles.Administrator)]
public record DeleteSupplierCommand : IRequest
{
    public Guid Id { get; init; }
}
