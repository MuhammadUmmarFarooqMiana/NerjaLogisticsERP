using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocumentFolders;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record GetCompanyDocumentFoldersQuery : IRequest<List<CompanyDocumentFolderDto>>
{
    /// <summary>Null returns the root-level folders.</summary>
    public Guid? ParentFolderId { get; init; }
}
