using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocuments;

public record GetEmployeeDocumentsQuery : IRequest<List<EmployeeDocumentDto>>
{
    public Guid? EmployeeId { get; init; }
    public Guid? PlatformId { get; init; }
}
