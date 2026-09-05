using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.CreateCompanyDocumentFolder;

// Folders are shared/company-wide — anyone with module access can create one
// at any level, unlike documents which stay filtered per-uploader.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record CreateCompanyDocumentFolderCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public Guid? ParentFolderId { get; init; }
}
