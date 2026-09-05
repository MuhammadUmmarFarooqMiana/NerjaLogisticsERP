using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.DeleteCompanyDocumentFolder;

// Administrator-only — a shared folder can hold documents uploaded by anyone,
// so deleting one is an oversight action, not something every uploader should do.
[Authorize(Roles = Roles.Administrator)]
public record DeleteCompanyDocumentFolderCommand : IRequest
{
    public Guid Id { get; init; }
}
