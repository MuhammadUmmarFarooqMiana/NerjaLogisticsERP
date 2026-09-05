using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentById;

[Authorize]
public record GetEmployeeDocumentByIdQuery : IRequest<EmployeeDocumentDto>
{
    public Guid Id { get; init; }
}

