using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;

public record UploadCompanyDocumentCommand : IRequest<Guid>
{
    public string Title { get; init; } = string.Empty;
    public CompanyDocumentCategory Category { get; init; }
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
