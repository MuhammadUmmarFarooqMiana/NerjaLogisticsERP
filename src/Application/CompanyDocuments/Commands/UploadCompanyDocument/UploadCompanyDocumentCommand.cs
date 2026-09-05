using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record UploadCompanyDocumentCommand : IRequest<Guid>
{
    public string Title { get; init; } = string.Empty;
    public CompanyDocumentCategory Category { get; init; }
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    /// <summary>Null uploads to the root, outside any folder.</summary>
    public Guid? FolderId { get; init; }
}
