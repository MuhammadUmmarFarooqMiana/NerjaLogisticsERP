using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;

[Authorize(Roles = Roles.Administrator)]
public class UploadCompanyDocumentCommandHandler : IRequestHandler<UploadCompanyDocumentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _currentUser;

    public UploadCompanyDocumentCommandHandler(IApplicationDbContext context, IFileStorageService storage, IUser currentUser)
    { _context = context; _storage = storage; _currentUser = currentUser; }

    public async Task<Guid> Handle(UploadCompanyDocumentCommand request, CancellationToken cancellationToken)
    {
        var storageKey = await _storage.SaveAsync(request.Content, request.FileName, "company", cancellationToken);
        var doc = CompanyDocument.Create(request.Title, request.Category, storageKey, request.FileName, request.ContentType, _currentUser.Id!.Value);
        _context.CompanyDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);
        return doc.Id;
    }
}
