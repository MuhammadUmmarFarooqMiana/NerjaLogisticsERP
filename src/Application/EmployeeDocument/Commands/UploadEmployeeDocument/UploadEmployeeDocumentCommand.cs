using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Commands.UploadEmployeeDocument;

[Authorize]
public record UploadEmployeeDocumentCommand : IRequest<Guid>
{
    /// <summary>
    /// Only honored for Administrator/Supervisor callers (Supervisor must own the target employee).
    /// Any other caller always uploads to their own resolved Employee record.
    /// </summary>
    public Guid? EmployeeId { get; init; }
    public EmployeeDocumentType Type { get; init; }
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
