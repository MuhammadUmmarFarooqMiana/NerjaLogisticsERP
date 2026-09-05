using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocumentFolders;

public class GetCompanyDocumentFoldersQueryHandler : IRequestHandler<GetCompanyDocumentFoldersQuery, List<CompanyDocumentFolderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompanyDocumentFoldersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CompanyDocumentFolderDto>> Handle(GetCompanyDocumentFoldersQuery request, CancellationToken cancellationToken)
        => await _context.CompanyDocumentFolders
            .Where(f => f.ParentFolderId == request.ParentFolderId)
            .OrderBy(f => f.Name)
            .Select(f => new CompanyDocumentFolderDto
            {
                Id = f.Id,
                Name = f.Name,
                ParentFolderId = f.ParentFolderId,
                CreatedByName = f.CreatedBy != null
                    ? _context.Employees.Where(e => e.UserId == f.CreatedBy).Select(e => e.FullName).FirstOrDefault()
                    : null,
                Created = f.Created
            })
            .ToListAsync(cancellationToken);
}
