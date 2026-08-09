using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentFile;

public class GetEmployeeDocumentFileQueryHandler : IRequestHandler<GetEmployeeDocumentFileQuery, DocumentFileResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;

    public GetEmployeeDocumentFileQueryHandler(IApplicationDbContext context, IFileStorageService storage)
    { _context = context; _storage = storage; }

    public async Task<DocumentFileResult> Handle(GetEmployeeDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var employeeDocuments = await _context.EmployeeDocuments.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeDocument), request.Id.ToString());

        var content = await _storage.GetAsync(employeeDocuments.StorageKey, cancellationToken)
            ?? throw new NotFoundException("File", employeeDocuments.StorageKey);

        return new DocumentFileResult(content, employeeDocuments.ContentType, employeeDocuments.OriginalFileName);
    }
}
