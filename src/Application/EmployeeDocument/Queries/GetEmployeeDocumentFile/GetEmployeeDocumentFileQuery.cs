
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentFile;

public record GetEmployeeDocumentFileQuery : IRequest<DocumentFileResult> { public Guid Id { get; init; } }

