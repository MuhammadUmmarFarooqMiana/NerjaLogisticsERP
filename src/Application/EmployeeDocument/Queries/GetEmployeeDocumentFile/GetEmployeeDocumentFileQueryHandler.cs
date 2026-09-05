using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentFile;

public class GetEmployeeDocumentFileQueryHandler : IRequestHandler<GetEmployeeDocumentFileQuery, DocumentFileResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _currentUser;

    public GetEmployeeDocumentFileQueryHandler(IApplicationDbContext context, IFileStorageService storage, IUser currentUser)
    { _context = context; _storage = storage; _currentUser = currentUser; }

    public async Task<DocumentFileResult> Handle(GetEmployeeDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.EmployeeDocuments
            .Where(d => d.Id == request.Id)
            .Select(d => new
            {
                Document = d,
                OwnerUserId = d.Employee.UserId,
                SupervisorUserId = d.Employee.Supervisor != null ? d.Employee.Supervisor.UserId : (Guid?)null
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeDocument), request.Id.ToString());

        var userId = _currentUser.Id!.Value;
        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator && result.OwnerUserId != userId && result.SupervisorUserId != userId)
            throw new ForbiddenAccessException();

        var content = await _storage.GetAsync(result.Document.StorageKey, cancellationToken)
            ?? throw new NotFoundException("File", result.Document.StorageKey);

        return new DocumentFileResult(content, result.Document.ContentType, result.Document.OriginalFileName);
    }
}
