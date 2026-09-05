using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetEmployeeDocumentById;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record GetCompanyDocumentByIdQuery : IRequest<CompanyDocumentDto> { public Guid Id { get; init; } }

