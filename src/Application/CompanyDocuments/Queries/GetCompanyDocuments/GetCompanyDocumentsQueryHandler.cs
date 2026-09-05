using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocuments;

public class GetCompanyDocumentsQueryHandler : IRequestHandler<GetCompanyDocumentsQuery, PaginatedList<CompanyDocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public GetCompanyDocumentsQueryHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task<PaginatedList<CompanyDocumentDto>> Handle(GetCompanyDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CompanyDocuments.Where(d => d.FolderId == request.FolderId);

        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator)
            query = query.Where(d => d.UploadedBy == _currentUser.Id);

        return await query
            .OrderByDescending(d => d.Created)
            .Select(d => new CompanyDocumentDto
            {
                Id = d.Id,
                Title = d.Title,
                Category = d.Category.ToString(),
                OriginalFileName = d.OriginalFileName,
                FolderId = d.FolderId,
                UploadedByName = _context.Employees.Where(e => e.UserId == d.UploadedBy).Select(e => e.FullName).FirstOrDefault(),
                Created = d.Created
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
