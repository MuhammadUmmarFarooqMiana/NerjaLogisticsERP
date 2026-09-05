
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentFile;

[Authorize]
public record GetEmployeeDocumentFileQuery : IRequest<DocumentFileResult> { public Guid Id { get; init; } }

