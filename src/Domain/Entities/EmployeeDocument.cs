namespace NerjaLogisticsERP.Domain.Entities;

public class EmployeeDocument : BaseAuditableEntity
{
    private EmployeeDocument() { }
    private EmployeeDocument(Guid employeeId, EmployeeDocumentType type, string storageKey, string originalFileName, string contentType, Guid uploadedBy)
    {
        EmployeeId = employeeId;
        Type = type;
        StorageKey = storageKey;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        UploadedBy = uploadedBy;
    }

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    public EmployeeDocumentType Type { get; private set; }
    public string StorageKey { get; private set; } = default!;
    public string OriginalFileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public Guid UploadedBy { get; private set; }

    public static EmployeeDocument Create(Guid employeeId, EmployeeDocumentType type, string storageKey, string originalFileName, string contentType, Guid uploadedBy)
        => new(employeeId, type, storageKey, originalFileName, contentType, uploadedBy);
}
