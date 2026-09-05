using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.DeleteCompanyDocument;

public class DeleteCompanyDocumentCommandHandler : IRequestHandler<DeleteCompanyDocumentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;

    public DeleteCompanyDocumentCommandHandler(IApplicationDbContext context, IFileStorageService storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task Handle(DeleteCompanyDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = await _context.CompanyDocuments.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(CompanyDocument), request.Id.ToString());

        // Best-effort — a missing/already-gone file on disk shouldn't block
        // removing the (now orphaned anyway) database record.
        await _storage.DeleteAsync(doc.StorageKey, cancellationToken);

        _context.CompanyDocuments.Remove(doc);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
