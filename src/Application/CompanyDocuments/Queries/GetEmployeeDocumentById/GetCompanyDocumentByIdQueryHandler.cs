using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetEmployeeDocumentById;

public class GetCompanyDocumentByIdQueryHandler : IRequestHandler<GetCompanyDocumentByIdQuery, CompanyDocumentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public GetCompanyDocumentByIdQueryHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CompanyDocumentDto> Handle(GetCompanyDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var doc = await _context.CompanyDocuments.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(CompanyDocument), request.Id.ToString());

        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator && doc.UploadedBy != _currentUser.Id)
            throw new ForbiddenAccessException();

        return new CompanyDocumentDto
        {
            Id = doc.Id,
            Title = doc.Title,
            Category = doc.Category.ToString(),
            OriginalFileName = doc.OriginalFileName,
            FolderId = doc.FolderId,
            UploadedByName = await _context.Employees.Where(e => e.UserId == doc.UploadedBy).Select(e => e.FullName).FirstOrDefaultAsync(cancellationToken),
            Created = doc.Created
        };
    }
}
