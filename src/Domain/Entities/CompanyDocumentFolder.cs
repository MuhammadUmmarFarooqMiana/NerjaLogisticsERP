namespace NerjaLogisticsERP.Domain.Entities;

// Shared/company-wide — every role with module access sees the same folder
// tree (unlike CompanyDocument, which is filtered per-uploader for non-Admins).
public class CompanyDocumentFolder : BaseAuditableEntity
{
    private CompanyDocumentFolder() { }
    private CompanyDocumentFolder(string name, Guid? parentFolderId)
    {
        Name = name;
        ParentFolderId = parentFolderId;
    }

    public string Name { get; private set; } = default!;
    public Guid? ParentFolderId { get; private set; }
    public CompanyDocumentFolder? ParentFolder { get; private set; }

    public static CompanyDocumentFolder Create(string name, Guid? parentFolderId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Folder name is required.", nameof(name));

        return new CompanyDocumentFolder(name, parentFolderId);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Folder name is required.", nameof(name));

        Name = name;
    }
}
