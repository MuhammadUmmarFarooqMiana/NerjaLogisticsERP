using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.DeleteCompanyDocumentFolder;

public class DeleteCompanyDocumentFolderCommandHandler : IRequestHandler<DeleteCompanyDocumentFolderCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCompanyDocumentFolderCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteCompanyDocumentFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await _context.CompanyDocumentFolders.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(CompanyDocumentFolder), request.Id.ToString());

        var hasSubfolders = await _context.CompanyDocumentFolders.AnyAsync(f => f.ParentFolderId == request.Id, cancellationToken);
        if (hasSubfolders) throw new ConflictException("Cannot delete a folder that still has subfolders.");

        var hasDocuments = await _context.CompanyDocuments.AnyAsync(d => d.FolderId == request.Id, cancellationToken);
        if (hasDocuments) throw new ConflictException("Cannot delete a folder that still has documents.");

        _context.CompanyDocumentFolders.Remove(folder);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
