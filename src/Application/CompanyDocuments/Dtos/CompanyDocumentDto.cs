namespace NerjaLogisticsERP.Application.CompanyDocuments.Dtos;

public record CompanyDocumentDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Category { get; init; } = default!;
    public string OriginalFileName { get; init; } = default!;
    public Guid? FolderId { get; init; }
    public string? UploadedByName { get; init; }
    public DateTimeOffset Created { get; init; }
}
