using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;

public class UploadCompanyDocumentCommandHandler : IRequestHandler<UploadCompanyDocumentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _currentUser;

    public UploadCompanyDocumentCommandHandler(IApplicationDbContext context, IFileStorageService storage, IUser currentUser)
    { _context = context; _storage = storage; _currentUser = currentUser; }

    public async Task<Guid> Handle(UploadCompanyDocumentCommand request, CancellationToken cancellationToken)
    {
        if (request.FolderId.HasValue)
        {
            var folderExists = await _context.CompanyDocumentFolders.AnyAsync(f => f.Id == request.FolderId, cancellationToken);
            if (!folderExists) throw new NotFoundException(nameof(CompanyDocumentFolder), request.FolderId.Value.ToString());
        }

        var storageKey = await _storage.SaveAsync(request.Content, request.FileName, "company", cancellationToken);
        var doc = CompanyDocument.Create(request.Title, request.Category, storageKey, request.FileName, request.ContentType, _currentUser.Id!.Value, request.FolderId);
        _context.CompanyDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);
        return doc.Id;
    }
}
