namespace NerjaLogisticsERP.Application.EmployeeDocument.Dtos;

public record EmployeeDocumentDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = default!;
    public string Type { get; init; } = default!;
    public string OriginalFileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public DateTimeOffset Created { get; init; }
}
