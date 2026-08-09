using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetEmployeeDocumentById;

[Authorize(Roles = Roles.Administrator)]
public class GetCompanyDocumentByIdQueryHandler : IRequestHandler<GetCompanyDocumentByIdQuery, CompanyDocumentDto>
{
    private readonly IApplicationDbContext _context;

    public GetCompanyDocumentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyDocumentDto> Handle(GetCompanyDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var companyDocuments = await _context.CompanyDocuments
            .Where(d => d.Id == request.Id)
            .Select(d => new CompanyDocumentDto
            {
                Id = d.Id,
                Title = d.Title,
                Category = d.Category.ToString(),
                OriginalFileName = d.OriginalFileName,
                Created = d.Created
            })
            .FirstOrDefaultAsync(cancellationToken);

        return companyDocuments ?? throw new NotFoundException(nameof(CompanyDocument), request.Id.ToString());
    }
}
