using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Common.Models;


[Authorize(Roles = Roles.Administrator)]
public class GetCompanyDocumentFileQueryHandler : IRequestHandler<GetCompanyDocumentFileQuery, DocumentFileResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;

    public GetCompanyDocumentFileQueryHandler(IApplicationDbContext context, IFileStorageService storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<DocumentFileResult> Handle(GetCompanyDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var doc = await _context.CompanyDocuments.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(CompanyDocument), request.Id.ToString());

        var content = await _storage.GetAsync(doc.StorageKey, cancellationToken)
            ?? throw new NotFoundException("File", doc.StorageKey);

        return new DocumentFileResult(content, doc.ContentType, doc.OriginalFileName);
    }
}
