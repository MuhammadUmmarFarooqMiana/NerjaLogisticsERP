using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetEmployeeDocumentById;

public record GetCompanyDocumentByIdQuery : IRequest<CompanyDocumentDto> { public Guid Id { get; init; } }

