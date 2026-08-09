namespace NerjaLogisticsERP.Domain.Entities;

public class CompanyDocument : BaseAuditableEntity
{
    private CompanyDocument() { }
    private CompanyDocument(string title, CompanyDocumentCategory category, string storageKey, string originalFileName, string contentType, Guid uploadedBy)
    {
        Title = title;
        Category = category;
        StorageKey = storageKey;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        UploadedBy = uploadedBy;
    }

    public string Title { get; private set; } = default!;
    public CompanyDocumentCategory Category { get; private set; }
    public string StorageKey { get; private set; } = default!;
    public string OriginalFileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public Guid UploadedBy { get; private set; }

    public static CompanyDocument Create(string title, CompanyDocumentCategory category, string storageKey, string originalFileName, string contentType, Guid uploadedBy)
        => new(title, category, storageKey, originalFileName, contentType, uploadedBy);
}
