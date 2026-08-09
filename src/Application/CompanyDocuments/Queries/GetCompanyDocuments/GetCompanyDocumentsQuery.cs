using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocuments;

public record GetCompanyDocumentsQuery : IRequest<List<CompanyDocumentDto>>;
