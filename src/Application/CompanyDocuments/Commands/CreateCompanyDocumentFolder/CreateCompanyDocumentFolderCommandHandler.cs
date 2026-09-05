using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.CreateCompanyDocumentFolder;

public class CreateCompanyDocumentFolderCommandHandler : IRequestHandler<CreateCompanyDocumentFolderCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyDocumentFolderCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateCompanyDocumentFolderCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentFolderId.HasValue)
        {
            var parentExists = await _context.CompanyDocumentFolders.AnyAsync(f => f.Id == request.ParentFolderId, cancellationToken);
            if (!parentExists) throw new NotFoundException(nameof(CompanyDocumentFolder), request.ParentFolderId.Value.ToString());
        }

        var folder = CompanyDocumentFolder.Create(request.Name, request.ParentFolderId);
        _context.CompanyDocumentFolders.Add(folder);
        await _context.SaveChangesAsync(cancellationToken);
        return folder.Id;
    }
}
