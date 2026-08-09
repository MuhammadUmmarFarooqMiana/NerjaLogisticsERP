using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Commands.UploadEmployeeDocument;

public record UploadEmployeeDocumentCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public EmployeeDocumentType Type { get; init; }
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
