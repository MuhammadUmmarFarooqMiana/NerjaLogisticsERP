using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Common.Models;

public class GetCompanyDocumentFileQueryHandler : IRequestHandler<GetCompanyDocumentFileQuery, DocumentFileResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _currentUser;

    public GetCompanyDocumentFileQueryHandler(IApplicationDbContext context, IFileStorageService storage, IUser currentUser)
    {
        _context = context;
        _storage = storage;
        _currentUser = currentUser;
    }

    public async Task<DocumentFileResult> Handle(GetCompanyDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var doc = await _context.CompanyDocuments.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(CompanyDocument), request.Id.ToString());

        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator && doc.UploadedBy != _currentUser.Id)
            throw new ForbiddenAccessException();

        var content = await _storage.GetAsync(doc.StorageKey, cancellationToken)
            ?? throw new NotFoundException("File", doc.StorageKey);

        return new DocumentFileResult(content, doc.ContentType, doc.OriginalFileName);
    }
}
