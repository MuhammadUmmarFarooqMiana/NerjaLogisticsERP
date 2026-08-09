using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentById;

public record GetEmployeeDocumentByIdQuery : IRequest<EmployeeDocumentDto> 
{ 
    public Guid Id { get; init; }
}

