using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocuments;

[Authorize(Roles = Roles.Administrator)]
public class GetCompanyDocumentsQueryHandler : IRequestHandler<GetCompanyDocumentsQuery, List<CompanyDocumentDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCompanyDocumentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CompanyDocumentDto>> Handle(GetCompanyDocumentsQuery request, CancellationToken cancellationToken)
        => await _context.CompanyDocuments
            .OrderByDescending(d => d.Created)
            .Select(d => new CompanyDocumentDto { Id = d.Id, Title = d.Title, Category = d.Category.ToString(), OriginalFileName = d.OriginalFileName, Created = d.Created })
            .ToListAsync(cancellationToken);
}
