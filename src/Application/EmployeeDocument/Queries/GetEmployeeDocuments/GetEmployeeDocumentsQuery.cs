using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocuments;

[Authorize]
public record GetEmployeeDocumentsQuery : IRequest<List<EmployeeDocumentDto>>
{
    public Guid? EmployeeId { get; init; }
    public Guid? PlatformId { get; init; }
}
