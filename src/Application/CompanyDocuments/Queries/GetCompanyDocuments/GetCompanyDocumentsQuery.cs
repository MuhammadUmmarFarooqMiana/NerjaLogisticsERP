using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocuments;

// Administrator sees every uploader's documents in the folder; Supervisor and
// Accountant only see their own (enforced in the handler) — the folder tree
// itself is shared and visible to all three regardless.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record GetCompanyDocumentsQuery : IRequest<PaginatedList<CompanyDocumentDto>>
{
    /// <summary>Null returns documents at the root, outside any folder.</summary>
    public Guid? FolderId { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
