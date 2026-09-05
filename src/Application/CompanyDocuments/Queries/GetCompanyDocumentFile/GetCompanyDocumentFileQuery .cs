using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Common.Models;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record GetCompanyDocumentFileQuery : IRequest<DocumentFileResult>
{
    public Guid Id { get; init; }
}
