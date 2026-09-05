namespace NerjaLogisticsERP.Application.CompanyDocuments.Dtos;

public record CompanyDocumentFolderDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public Guid? ParentFolderId { get; init; }
    public string? CreatedByName { get; init; }
    public DateTimeOffset Created { get; init; }
}
